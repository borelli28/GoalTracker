import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { ContributionCalendar } from 'react-contribution-calendar';

const API_URL = import.meta.env.VITE_API_URL;

const ProgressGrid = ({ goalId }) => {
  const [progressData, setProgressData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  useEffect(() => {
    fetchProgressData();
  }, [goalId]);

  const fetchProgressData = async () => {
    try {
      setLoading(true);
      
      const endDate = new Date();
      const startDate = new Date(endDate);
      startDate.setMonth(startDate.getMonth() - 2);
      const response = await axios.get(`${API_URL}/api/Progress/goal/${goalId}?startDate=${startDate.toISOString()}`);

      if (response.data && Array.isArray(response.data.$values)) {
        const formattedData = formatDataToLevels(response.data.$values);
        setProgressData(formattedData);
      } else {
        setError('Received unexpected data format from server.');
      }
    } catch (err) {
      setError('Failed to fetch progress data.');
    } finally {
      setLoading(false);
    }
  };

  const formatDataToLevels = (data) => {
    return [data.reduce((acc, item) => {
      const date = new Date(item.date).toISOString().split('T')[0];
      acc[date] = { level: item.completed ? 3 : 0 };
      return acc;
    }, {})];
  };

  const handleCellClick = (e, data) => {
    console.log('Cell clicked:', data);
  };  

  if (loading) {
    return <div>Loading progress...</div>;
  }

  if (error) {
    return <div className="text-red-500">{error}</div>;
  }

  const startDate = new Date();
  startDate.setMonth(startDate.getMonth() - 2);

  return (
    <div>
      <ContributionCalendar
        data={progressData}
        start={startDate.toISOString().split('T')[0]} // Start date
        end={new Date().toISOString().split('T')[0]} // End date (today)
        daysOfTheWeek={['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']}
        textColor="#1F2328"
        startsOnSunday={true}
        includeBoundary={true}
        theme="cherry"
        cx={10}
        cy={10}
        cr={2}
        onCellClick={handleCellClick}
        scroll={false}
      />
    </div>
  );
};

export default ProgressGrid;
