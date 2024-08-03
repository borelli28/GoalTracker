import React, { useState } from 'react';
import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL;

const CreateGoal = ({ onGoalCreated }) => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess(false);

    try {
      const response = await axios.post(`${API_URL}/api/Goal`, {
        name,
        description
      });

      if (response.status === 200) {
        setSuccess(true);
        setName('');
        setDescription('');
        onGoalCreated(response.data);
      }
    } catch (err) {
      setError('Failed to create goal. Please try again.');
    }
  };

  return (
    <div>
      <h2>Create New Goal</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {success && <p style={{ color: 'green' }}>Goal created successfully!</p>}
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="name">Name:</label>
          <input
            type="text"
            id="name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
            maxLength={30}
          />
        </div>
        <div>
          <label htmlFor="description">Description:</label>
          <textarea
            id="description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            maxLength={90}
          />
        </div>
        <button type="submit">Create Goal</button>
      </form>
    </div>
  );
};

export default CreateGoal;
