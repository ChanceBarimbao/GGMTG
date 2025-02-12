import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Login.css';

function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate(); // To navigate to another page

    const handleSubmit = (e) => {
        e.preventDefault();
        fetch('https://localhost:7212/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ email, password }),
        })
            .then((res) => res.json())
            .then((data) => {
                if (data.message === 'Login successful') {
                    // Save email to localStorage (or use state)
                    localStorage.setItem('email', email);
                    navigate('/search');  // Redirect to sets page
                } else {
                    setError('Login failed');
                }
            })
            .catch((err) => setError('Login error: ' + err.message));
    };

    return (
        <div className="container">
            <form onSubmit={handleSubmit}>
                <h2>Login</h2>
                <div>
                    <label>Email:</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Password:</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <button type="submit">Login</button>
            </form>
            {error && <p className="error-message">{error}</p>}
        </div>
    );
}

export default Login;
