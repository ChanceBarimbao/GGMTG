import { useState } from 'react';
import './SignUp.css';

function SignUp() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [reEnterPassword, setReEnterPassword] = useState('');
    const [loading, setLoading] = useState(false); // For loading state
    const [error, setError] = useState(''); // For error messages
    const [success, setSuccess] = useState(false); // For success message

    // Handle form submission
    const handleSubmit = async (e) => {
        e.preventDefault();

        // Basic password match validation
        if (password !== reEnterPassword) {
            setError("Passwords do not match!");
            return;
        }

        // Basic validation for email and password (you can add more here)
        if (!email || !password) {
            setError("Email and password are required.");
            return;
        }

        setLoading(true); // Start loading

        try {
            // Making POST request to the backend API (replace with your actual API endpoint)
            const response = await fetch('https:////localhost:5296/api/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    email,
                    password,
                }),
            });

            const data = await response.json();

            // Check if the API returned a successful response
            if (!response.ok) {
                // If the response is not OK, display the error message
                setLoading(false);
                setError(data.message || "An error occurred. Please try again.");
                return;
            }

            // If successful, show success message and reset form
            setLoading(false);
            setSuccess(true);
            setError('');
            setEmail('');
            setPassword('');
            setReEnterPassword('');

            // Optionally, you can redirect to the login page or another page
            // window.location.href = '/login'; // For example, redirect to login page

        } catch (err) {
            // Catch any errors from the fetch call
            setLoading(false);
            setError("Something went wrong. Please try again later.");
            console.error("Error during sign-up:", err);
        }
    };

    return (
        <div className="container">
            <form onSubmit={handleSubmit}>
                <h2>Sign Up</h2>

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

                <div>
                    <label>Re-enter Password:</label>
                    <input
                        type="password"
                        value={reEnterPassword}
                        onChange={(e) => setReEnterPassword(e.target.value)}
                        required
                    />
                </div>

                {/* Show error message */}
                {error && <p style={{ color: 'red' }}>{error}</p>}

                {/* Show success message */}
                {success && <p style={{ color: 'green' }}>Sign-up successful!</p>}

                <button type="submit" disabled={loading || password !== reEnterPassword}>
                    {loading ? "Signing up..." : "Sign Up"}
                </button>
            </form>
        </div>
    );
}

export default SignUp;
