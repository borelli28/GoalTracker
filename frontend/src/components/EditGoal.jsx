import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL;

const EditGoal = ({ onGoalUpdated }) => {
  const { id } = useParams();
  const [goal, setGoal] = useState({ name: '', description: '' });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    fetchGoal();
  }, [id]);

  const fetchGoal = async () => {
    try {
      setLoading(true);
      const response = await axios.get(`${API_URL}/api/Goal/${id}`);
      setGoal(response.data);
      setError('');
    } catch (err) {
      setError('Failed to fetch goal. Please try again.');
      console.error('Error fetching goal:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess(false);

    try {
      const response = await axios.put(`${API_URL}/api/Goal/${id}`, goal);

      if (response.status === 204) {
        setSuccess(true);
        if (onGoalUpdated) onGoalUpdated(goal);
      }
    } catch (err) {
      setError('Failed to update goal. Please try again.');
      console.error('Error updating goal:', err);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setGoal((prevGoal) => ({
      ...prevGoal,
      [name]: value,
    }));
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      <h2>Edit Goal</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {success && <p style={{ color: 'green' }}>Goal updated successfully!</p>}
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="name">Name:</label>
          <input
            type="text"
            id="name"
            name="name"
            value={goal.name}
            onChange={handleChange}
            required
            maxLength={30}
          />
        </div>
        <div>
          <label htmlFor="description">Description:</label>
          <textarea
            id="description"
            name="description"
            value={goal.description}
            onChange={handleChange}
            maxLength={90}
          />
        </div>
        <button type="submit">Update Goal</button>
      </form>
    </div>
  );
};

export default EditGoal;
