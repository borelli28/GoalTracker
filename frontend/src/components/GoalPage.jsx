import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import axios from 'axios';
import { ContributionCalendar } from 'react-contribution-calendar';

const API_URL = import.meta.env.VITE_API_URL;

const GoalPage = () => {
  const { id } = useParams();
  const [goal, setGoal] = useState(null);
  const [progressData, setProgressData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [updateError, setUpdateError] = useState(null);

  useEffect(() => {
    fetchGoalAndProgress();
  }, [id]);

  const fetchGoalAndProgress = async () => {
    try {
      setLoading(true);
      setError(null);
      const [goalResponse, progressResponse] = await Promise.all([
        axios.get(`${API_URL}/api/Goal/${id}`),
        fetchProgressData()
      ]);

      setGoal(goalResponse.data);
      setProgressData(progressResponse);
    } catch (err) {
      setError('Failed to fetch goal data. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const fetchProgressData = async () => {
    const endDate = new Date();
    const startDate = new Date(endDate);
    startDate.setMonth(startDate.getMonth() - 12);
    
    const response = await axios.get(`${API_URL}/api/Progress/goal/${id}?startDate=${startDate.toISOString()}`);
    
    if (response.data && Array.isArray(response.data.$values)) {
      return formatDataToLevels(response.data.$values);
    } else {
      throw new Error('Unexpected data format from server.');
    }
  };

  const formatDataToLevels = (data) => {
    return [data.reduce((acc, item) => {
      const date = new Date(item.date).toISOString().split('T')[0];
      acc[date] = { level: item.completed ? 3 : 0, id: item.id };
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
      setUpdateError(null);
      const response = await axios.put(`${API_URL}/api/Progress`, {
        id: currentData.id,
        date: date,
        completed: updatedCompleted,
        goalId: id
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
      setUpdateError('Failed to update progress. Please try again.');
      // Auto-hide the error message after 5 seconds
      setTimeout(() => setUpdateError(null), 5000);
    }
  };

  if (loading) return <div className="text-center py-4">Loading...</div>;
  if (error) return <div className="text-red-500 text-center">{error}</div>;
  if (!goal) return <div className="text-center py-4">Goal not found</div>;

  const startDate = new Date();
  startDate.setMonth(startDate.getMonth() - 12);

  return (
    <div className="max-w-4xl mx-auto p-4">
      <Link to="/" className="text-indigo-600 hover:text-indigo-800 mb-4 inline-block">&larr; Back to Goals</Link>
      <h1 className="text-3xl font-bold mb-2">{goal.name}</h1>
      <p className="text-gray-600 mb-6">{goal.description}</p>

      <h2 className="text-2xl font-semibold mb-4">Progress (Last 12 Months)</h2>
      {updateError && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative mb-4" role="alert">
          <strong className="font-bold">Error: </strong>
          <span className="block sm:inline">{updateError}</span>
        </div>
      )}
      <div className="bg-white shadow-md rounded-lg p-6"> {/* White card styling */}
        <ContributionCalendar
          data={progressData}
          start={startDate.toISOString().split('T')[0]} // Start date
          end={new Date().toISOString().split('T')[0]} // End date (today)
          daysOfTheWeek={['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']}
          textColor="#1F2328"
          startsOnSunday={true}
          includeBoundary={true}
          theme="vomit"
          cx={14}
          cy={14}
          cr={2}
          onCellClick={handleCellClick}
          scroll={false}
        />
      </div>
    </div>
  );
};

export default GoalPage;
