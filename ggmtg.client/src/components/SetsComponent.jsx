import { useEffect, useState } from 'react';
import './Sets.css';

function ScryfallSetsComponent() {
    const [allSets, setAllSets] = useState([]);  // All sets fetched from Scryfall
    const [cards, setCards] = useState([]);      // Cards from selected set
    const [searchCode, setSearchCode] = useState(''); // Search code entered by user
    const [loadingSets, setLoadingSets] = useState(true); // Loading state for sets
    const [loadingCards, setLoadingCards] = useState(false); // Loading state for cards
    const [error, setError] = useState('');    // Error handling

    // Fetch all Scryfall sets on mount
    useEffect(() => {
        fetch('https://api.scryfall.com/sets')
            .then((res) => res.json())
            .then((data) => {
                if (data.data) {
                    setAllSets(data.data); // Set fetched sets
                } else {
                    setError('Unexpected data structure');
                }
            })
            .catch((err) => setError('Error fetching sets: ' + err.message))
            .finally(() => setLoadingSets(false));
    }, []);

    // Fetch cards for a specific set code
    const handleGetCardsByCode = (code) => {
        setLoadingCards(true);
        const scryfallUri = `https://api.scryfall.com/cards/search?order=set&q=e%3A${code}&unique=prints`;
        fetch(scryfallUri)
            .then((res) => res.json())
            .then((data) => {
                if (data.data) {
                    setCards(data.data); // Set fetched cards
                } else {
                    setCards([]); // Empty result if no cards are found
                }
            })
            .catch((err) => setError('Error fetching cards: ' + err.message))
            .finally(() => setLoadingCards(false));
    };

    // Handle the form submit for searching cards
    const handleSearchSubmit = (e) => {
        e.preventDefault();
        if (!searchCode) {
            alert('Please enter a set code.');
            return;
        }
        handleGetCardsByCode(searchCode);
    };

    // Show a loading message if sets are still loading
    if (loadingSets) return <p>Loading sets...</p>;

    // If there's an error fetching sets or cards
    if (error) return <p className="error-message">Error: {error}</p>;

    return (
        <div className="container">
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

            {/* Main Content Area */}
            <div className="main-content">
                <h2>MTG Set Search</h2>

                {/* Search form */}
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

                {/* Loading spinner */}
                {loadingCards && <div className="loading-spinner"></div>}

                {/* Display fetched cards */}
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
