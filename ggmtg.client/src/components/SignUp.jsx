import  { useState } from 'react'
import './SignUp.css'
function SignUp() {
    //const [name, setName] = useState('')
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [reEnterPassword, setReEnterPassword] = useState("");
    //const handleSubmit = (e) => {
    //    e.preventDefault();
    //    // Add your sign-up logic here (e.g., API call)
    //    console.log('Sign Up Submitted:', { name, email, password })
    //};
    const handleSubmit = (e) => {
        e.preventDefault();
        if (password === reEnterPassword) {
            // Handle form submission logic here
            console.log("Form submitted");
        } else {
            // Show a message or handle the error
            alert("Passwords do not match!");
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
                {password !== reEnterPassword && reEnterPassword !== "" && (
                    <p style={{ color: 'red' }}>Passwords do not match!</p>
                )}
                <button type="submit" disabled={password !== reEnterPassword}>
                    Sign Up
                </button>
            </form>
        </div>
    );


}

export default SignUp