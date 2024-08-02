import React from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import Home from './components/Home';
import CreateGoalForm from './components/CreateGoal';
import EditGoalForm from './components/EditGoal';

const App = () => {
  return (
    <Router>
      {/* <nav>
        <ul>
          <li>
            <Link to="/">Home</Link>
          </li>
        </ul>
      </nav> */}
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/create" element={<CreateGoalForm />} />
        <Route path="/edit/:id" element={<EditGoalForm />} />
      </Routes>
    </Router>
  );
};

export default App;
