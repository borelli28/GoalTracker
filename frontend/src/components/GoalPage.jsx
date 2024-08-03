import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL;

const GoalPage = () => {
  const { id } = useParams();
  const [goal, setGoal] = useState(null);
  const [progressData, setProgressData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchGoalAndProgress();
  }, [id]);

  const fetchGoalAndProgress = async () => {
    try {
      setLoading(true);
      const [goalResponse, progressResponse] = await Promise.all([
        axios.get(`${API_URL}/api/Goal/${id}`),
        fetchProgressData()
      ]);

      setGoal(goalResponse.data);
      setProgressData(progressResponse);
    } catch (err) {
      console.error('Error fetching goal and progress:', err);
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
      return response.data.$values;
    } else {
      throw new Error('Unexpected data format from server.');
    }
  };

  const handleProgressToggle = async (date) => {
    try {
      const existingProgress = progressData.find(p => new Date(p.date).toDateString() === date.toDateString());
      
      const updatedProgress = {
        goalId: id,
        date: date.toISOString(),
        completed: existingProgress ? !existingProgress.completed : true
      };

      await axios.put(`${API_URL}/api/Progress`, updatedProgress);
      await fetchGoalAndProgress();
    } catch (err) {
      console.error('Error updating progress:', err);
      setError('Failed to update progress. Please try again.');
    }
  };

  const generateDateArray = () => {
    const endDate = new Date();
    const startDate = new Date(endDate);
    startDate.setMonth(startDate.getMonth() - 12);
    
    const dateArray = [];
    let currentDate = new Date(startDate);
    
    while (currentDate <= endDate) {
      dateArray.push(new Date(currentDate));
      currentDate.setDate(currentDate.getDate() + 1);
    }
    
    return dateArray;
  };

  const dateArray = generateDateArray();

  if (loading) return <div className="text-center py-4">Loading...</div>;
  if (error) return <div className="text-red-500 text-center">{error}</div>;
  if (!goal) return <div className="text-center py-4">Goal not found</div>;

  return (
    <div className="max-w-4xl mx-auto p-4">
      <Link to="/" className="text-indigo-600 hover:text-indigo-800 mb-4 inline-block">&larr; Back to Goals</Link>
      <h1 className="text-3xl font-bold mb-2">{goal.name}</h1>
      <p className="text-gray-600 mb-6">{goal.description}</p>

      <h2 className="text-2xl font-semibold mb-4">Progress (Last 12 Months)</h2>
      <div className="grid grid-cols-7 gap-1 mb-4">
        {['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'].map(day => (
          <div key={day} className="text-center text-xs font-medium text-gray-500">{day}</div>
        ))}
      </div>
      <div className="grid grid-cols-7 gap-1">
        {dateArray.map((date, index) => {
          const progressForDate = progressData.find(p => new Date(p.date).toDateString() === date.toDateString());
          return (
            <div
              key={index}
              className={`w-full pt-[100%] relative rounded cursor-pointer ${
                progressForDate?.completed ? 'bg-green-500' : 'bg-gray-200'
              }`}
              onClick={() => handleProgressToggle(date)}
              title={`${date.toLocaleDateString()}: ${progressForDate?.completed ? 'Completed' : 'Not completed'}`}
            >
              <span className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 text-xs">
                {date.getDate()}
              </span>
              <span className="absolute bottom-1 left-1/2 transform -translate-x-1/2 text-xs text-gray-600">
                {date.toLocaleString('default', { month: 'short' })}
              </span>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default GoalPage;
