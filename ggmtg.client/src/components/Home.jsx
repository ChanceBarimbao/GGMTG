import { useEffect } from 'react';
import { Link } from 'react-router-dom';
import './Home.css';

function Home() {
    useEffect(() => {
        localStorage.clear(); // Clear all local storage when the home page loads
    }, []);

    return (
        <div className="welcome-container">
            <div className="welcome-card">
                <h1 className="welcome-title">Welcome to <span className="brand-name">GGMTG</span></h1>
                <p className="welcome-description">Your gateway to the ultimate game-gambling experience.</p>
                <nav className="welcome-nav">
                    <Link to="/login" className="welcome-btn login-btn">Login</Link>
                    <Link to="/signup" className="welcome-btn signup-btn">Signup</Link>
                    <Link to="/search" className="Card view card-btn"> Continue as Guest</Link>
                </nav>
            </div>
        </div>
    );
}

export default Home;
