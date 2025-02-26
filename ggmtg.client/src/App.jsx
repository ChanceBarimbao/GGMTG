//import React front 'react'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Login from './components/Login.jsx'
import SignUp from './components/SignUp.jsx'
import Home from './components/Home.jsx'
import ScryfallSetsComponent from './components/SetsComponent.jsx'
import Card from './components/Card.jsx'

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Home/>} />
                <Route path="/login" element={<Login />} />
                <Route path="/signup" element={<SignUp />} />
                <Route path="/search" element={<ScryfallSetsComponent />} />
                <Route path="/card" element={<Card />} />

            </Routes>
        </Router>
    )
}

export default App;