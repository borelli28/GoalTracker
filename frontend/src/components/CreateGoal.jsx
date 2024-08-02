import React, { useState } from 'react';
import axios from 'axios';

const CreateGoalForm = () => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess(false);

    try {
      const response = await axios.post('http://localhost:5295/api/Goal', {
        name,
        description
      });

      if (response.status === 200) {
        setSuccess(true);
        setName('');
        setDescription('');
      }
    } catch (err) {
      setError('Failed to create goal. Please try again.');
      console.error('Error creating goal:', err);
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

export default CreateGoalForm;
