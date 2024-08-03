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
        setError('Received unexpected data format from server.');
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
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div>
      {goals.length === 0 ? (
        <div>
          <p>No goals found. Create your first goal!</p>
          <CreateGoalForm onGoalCreated={handleGoalCreated} />
        </div>
      ) : (
        <div>
          <h2>Your Goals</h2>
          <ul>
            {goals.map((goal) => (
              <li key={goal.id}>
                <h3>{goal.name}</h3>
                <p>{goal.description}</p>
                <ProgressGrid goalId={goal.id} />
                <Link to={`/edit/${goal.id}`}>Edit</Link>
                <button onClick={() => handleDeleteGoal(goal.id)}>Delete</button>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
};

export default Home;
