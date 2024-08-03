import React, { useState, useEffect } from 'react';
import axios from 'axios';

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

  const handleSquareClick = async (date) => {
    try {
      const existingProgress = progressData.find(p => new Date(p.date).toDateString() === date.toDateString());
      
      const updatedProgress = {
        goalId: goalId,
        date: date.toISOString(),
        // Sets completed to opposite. Allows to set/unset Progress.Completed OnClick
        completed: existingProgress ? !existingProgress.completed : true
      };

      await axios.put(`${API_URL}/api/Progress`, updatedProgress);

      // Refresh the progress data
      await fetchProgressData();
      setError(null); // Clear any previous errors
    } catch (err) {
      console.error('Error updating progress:', err.response || err.message || err);
      setError('Failed to update progress. Please try again.');
    }
  };

  const getColor = (completed) => {
    return completed ? '#196127' : '#ebedf0';
  };

  const generateDateArray = () => {
    const endDate = new Date();
    const startDate = new Date(endDate);
    startDate.setMonth(startDate.getMonth() - 2);
    
    const dateArray = [];
    let currentDate = new Date(startDate);
    
    while (currentDate <= endDate) {
      dateArray.push(new Date(currentDate));
      currentDate.setDate(currentDate.getDate() + 1);
    }
    
    return dateArray;
  };

  const dateArray = generateDateArray();

  const gridStyle = {
    display: 'grid',
    gridTemplateColumns: 'repeat(7, 1fr)',
    gap: '1px',
    maxWidth: '300px',
  };

  const squareStyle = {
    width: '100%',
    paddingBottom: '100%',
    position: 'relative',
    borderRadius: '2px',
    fontSize: '8px',
    color: '#333',
    cursor: 'pointer',
  };

  const contentStyle = {
    position: 'absolute',
    top: '0',
    left: '0',
    right: '0',
    bottom: '0',
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'center',
    alignItems: 'center',
    lineHeight: '1',
  };

  return (
    <div>
      {error && <div style={{ color: 'red', marginBottom: '10px' }}>{error}</div>}
      {loading && <div>Loading progress data...</div>}
      <div style={gridStyle}>
        {dateArray.map((date, index) => {
          const progressForDate = progressData.find(p => new Date(p.date).toDateString() === date.toDateString());
          return (
            <div
              key={index}
              style={{
                ...squareStyle,
                backgroundColor: getColor(progressForDate?.completed || false),
              }}
              onClick={() => handleSquareClick(date)}
              title={`Date: ${date.toLocaleDateString()}, Completed: ${progressForDate?.completed ? 'Yes' : 'No'}`}
            >
              <div style={contentStyle}>
                <div>{date.getDate()}</div>
                <div>{date.toLocaleString('default', { month: 'short' })}</div>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default ProgressGrid;
