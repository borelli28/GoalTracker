import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import Home from './components/Home';
import CreateGoalForm from './components/CreateGoal';
import EditGoalForm from './components/EditGoal';

const App = () => {
  return (
    <Router>
      <div className="min-h-screen bg-gray-100">
        <nav className="bg-white shadow-md">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="flex justify-between h-16">
              <div className="flex items-center">
                <Link to="/" className="text-xl font-bold text-indigo-600">GoalTracker</Link>
                <div className="ml-6 space-x-4">
                  <Link to="/" className="text-gray-500 hover:text-gray-800">Home</Link>
                  <Link to="/create" className="text-gray-500 hover:text-gray-800">Create Goal</Link>
                </div>
              </div>
            </div>
          </div>
        </nav>

        <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/create" element={<CreateGoalForm />} />
            <Route path="/edit/:id" element={<EditGoalForm />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
};

export default App;
