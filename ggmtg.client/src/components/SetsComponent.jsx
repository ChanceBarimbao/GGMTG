import { useEffect, useState } from 'react';
import './Sets.css';

function ScryfallSetsComponent() {
    const [allSets, setAllSets] = useState([]);
    const [cards, setCards] = useState([]);
    const [searchCode, setSearchCode] = useState('');
    const [loadingSets, setLoadingSets] = useState(true);
    const [loadingCards, setLoadingCards] = useState(false);
    const [error, setError] = useState('');

    const userEmail = localStorage.getItem('email');

    useEffect(() => {
        fetch('https://api.scryfall.com/sets')
            .then((res) => res.json())
            .then((data) => {
                if (data.data) {
                    setAllSets(data.data);
                } else {
                    setError('Unexpected data structure');
                }
            })
            .catch((err) => setError('Error fetching sets: ' + err.message))
            .finally(() => setLoadingSets(false));
    }, []);

    const handleGetCardsByCode = (code) => {
        setLoadingCards(true);
        const scryfallUri = `https://api.scryfall.com/cards/search?order=set&q=e%3A${code}&unique=prints`;
        fetch(scryfallUri)
            .then((res) => res.json())
            .then((data) => {
                if (data.data) {
                    setCards(data.data);
                } else {
                    setCards([]);
                }
            })
            .catch((err) => setError('Error fetching cards: ' + err.message))
            .finally(() => setLoadingCards(false));
    };

    const handleSearchSubmit = (e) => {
        e.preventDefault();
        if (!searchCode) {
            alert('Please enter a set code.');
            return;
        }
        handleGetCardsByCode(searchCode);
    };

    const handleButtonClick = () => {
        if (userEmail) {
            // Logout: clear the email from localStorage and reload the page
            localStorage.removeItem('email');
            window.location.reload();
        } else {
            // Home: you can modify this to redirect to a login page or home page
            window.location.href = '/'; // Redirect to home or login page
        }
    };

    if (loadingSets) return <p>Loading sets...</p>;
    if (error) return <p className="error-message">Error: {error}</p>;

    return (
        <div className="container">
            <div className="sidebar">
                {/* Display User's Email */}
                <div className="user-info">
                    <p>Logged in as: <strong>{userEmail || 'Guest'}</strong></p>
                </div>

                {/* Logout/Home Button */}
                <div className="logout-home-button">
                    <button onClick={handleButtonClick}>
                        {userEmail ? 'Logout' : 'Home'}
                    </button>
                </div>

                {/* Left Sidebar for Sets */}
                <div className="sets-sidebar">
                    <h3>All Scryfall Sets</h3>
                    {allSets.length === 0 ? (
                        <p>No sets available.</p>
                    ) : (
                        <ul className="sets-list">
                            {allSets.map((s) => (
                                <li key={s.id}>
                                    <code>{s.code}</code> → {s.name}
                                    <button
                                        onClick={() => handleGetCardsByCode(s.code)}
                                        className="show-cards-btn"
                                    >
                                        Show Cards
                                    </button>
                                </li>
                            ))}
                        </ul>
                    )}
                </div>
            </div>

            <div className="main-content">
                <h2>MTG Set Search</h2>
                <form onSubmit={handleSearchSubmit} className="search-form">
                    <input
                        type="text"
                        placeholder='e.g. "neo"'
                        value={searchCode}
                        onChange={(e) => setSearchCode(e.target.value.toLowerCase())}
                    />
                    <button type="submit" disabled={loadingCards}>
                        {loadingCards ? 'Searching...' : 'Fetch Single Set'}
                    </button>
                </form>

                {loadingCards && <div className="loading-spinner"></div>}

                <div className="search-results">
                    <h3>Fetched Cards:</h3>
                    {cards.length === 0 ? (
                        <p>No cards to display yet...</p>
                    ) : (
                        <ul>
                            {cards.map((card) => (
                                <li key={card.id}>
                                    <strong>{card.name}</strong>
                                    {card.type_line && ` | ${card.type_line}`}
                                    {card.oracle_text && ` | ${card.oracle_text}`}
                                </li>
                            ))}
                        </ul>
                    )}
                </div>
            </div>
        </div>
    );
}

export default ScryfallSetsComponent;
