import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { ContributionCalendar } from 'react-contribution-calendar'

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
        setProgressData(response.data.$values);
      } else {
        console.error('Unexpected data format:', response.data);
        setError('Received unexpected data format from server.');
      }
    } catch (err) {
      console.error('Error fetching progress data:', err);
      setError('Failed to fetch progress data.');
    } finally {
      setLoading(false);
    }
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

  return (
    <div>
      <ContributionCalendar
        data={progressData}
        start={new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0]} // Start of current year
        end={new Date().toISOString().split('T')[0]} // Today
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
