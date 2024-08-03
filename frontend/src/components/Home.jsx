import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';
import CreateGoalForm from './CreateGoal';
import ProgressGrid from './ProgressGrid';

const API_URL = import.meta.env.VITE_API_URL;

const Home = () => {
  const [goals, setGoals] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchGoals();
  }, []);

  const fetchGoals = async () => {
    try {
      setLoading(true);
      const response = await axios.get(`${API_URL}/api/Goal`);
      if (response.data && Array.isArray(response.data.$values)) {
        setGoals(response.data.$values);
      } else {
        setError('Unexpected data format from server.');
      }
    } catch (err) {
      setError('Failed to fetch goals. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleGoalCreated = (newGoal) => {
    setGoals(prevGoals => [...prevGoals, newGoal]);
  };

  const handleDeleteGoal = async (id) => {
    if (window.confirm('Are you sure you want to delete this goal?')) {
      try {
        await axios.delete(`${API_URL}/api/Goal/${id}`);
        setGoals(prevGoals => prevGoals.filter(goal => goal.id !== id));
      } catch (err) {
        setError('Failed to delete goal. Please try again.');
      }
    }
  };

  if (loading) {
    return <div className="text-center py-4">Loading...</div>;
  }

  if (error) {
    return <div className="text-red-500 text-center">{error}</div>;
  }

  return (
    <div className="bg-white shadow-md rounded-lg p-6">
      {goals.length === 0 ? (
        <div>
          <p className="text-lg text-gray-700 mb-4">No goals found. Create your first goal!</p>
          <CreateGoalForm onGoalCreated={handleGoalCreated} />
        </div>
      ) : (
        <div>
          <h2 className="text-2xl font-bold text-gray-900 mb-4">Your Goals</h2>
          <ul className="space-y-4">
            {goals.map((goal) => (
              <li key={goal.id} className="border-b pb-4">
                <h3 className="text-lg font-semibold text-gray-800">{goal.name}</h3>
                <ProgressGrid goalId={goal.id} />
                <div className="mt-2 flex space-x-3">
                  <Link to={`/goal/${goal.id}`} className="text-indigo-600 hover:text-indigo-800">View Goal</Link>
                  <Link to={`/edit/${goal.id}`} className="text-indigo-600 hover:text-indigo-800">Edit</Link>
                  <button onClick={() => handleDeleteGoal(goal.id)} className="text-red-600 hover:text-red-800">Delete</button>
                </div>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
};

export default Home;
