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

  const handleCellClick = async (e, cellData) => {
    const date = cellData.date;
    const currentData = progressData[0][date];
    
    if (!currentData) {
      return;
    }

    const updatedCompleted = currentData.level === 0;
    
    try {
      const response = await axios.put(`${API_URL}/api/Progress`, {
        id: currentData.id,
        date: date,
        completed: updatedCompleted,
        goalId: goalId
      });

      if (response.status === 204) {
        // Update the progress data for a specific date,
        // changing the level based on completion status
        setProgressData(prevData => [{
          ...prevData[0],
          [date]: {
            ...prevData[0][date],
            level: updatedCompleted ? 3 : 0
          }
        }]);
      }
    } catch (error) {
      console.error('Error updating progress:', error);
      setError('Failed to update progress. Please try again.');
    }
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
