import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './Sets.css';

function ScryfallSearchComponent() {
    const [cards, setCards] = useState([]);
    const [searchQuery, setSearchQuery] = useState('');
    const [searchType, setSearchType] = useState('name');
    const [loadingCards, setLoadingCards] = useState(false);
    const [error, setError] = useState('');
    const navigate = useNavigate();

    // Load last searched query from localStorage on mount
    useEffect(() => {
        const savedQuery = localStorage.getItem('lastSearchQuery');
        const savedType = localStorage.getItem('lastSearchType');

        if (savedQuery) {
            setSearchQuery(savedQuery);
            if (savedType) {
                setSearchType(savedType);
            }
            fetchCards(savedQuery, savedType || 'name'); // Fetch last searched cards
        }
    }, []);

    const handleSearchSubmit = (e) => {
        e.preventDefault();
        if (!searchQuery) {
            alert('Please enter a search term.');
            return;
        }

        // Save search query and type to localStorage
        localStorage.setItem('lastSearchQuery', searchQuery);
        localStorage.setItem('lastSearchType', searchType);

        fetchCards(searchQuery, searchType);
    };

    const fetchCards = (query, type) => {
        setLoadingCards(true);

        let searchParam = '';
        switch (type) {
            case 'name':
                searchParam = `name:${query}`;
                break;
            case 'type':
                searchParam = `type:${query}`;
                break;
            case 'description': // Search in card text (oracle)
            case 'keyword': // NEW: Search for keywords like Haste, Vigilance, Flying
                searchParam = `oracle:${query}`;
                break;
            default:
                searchParam = query;
        }

        const excludeUniquePrints = type === 'description' || type === 'keyword';
        const scryfallUri = `https://api.scryfall.com/cards/search?q=${encodeURIComponent(searchParam)}${excludeUniquePrints ? '' : '&unique=prints'}`;

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

    const handleCardClick = (card) => {
        localStorage.setItem('card', JSON.stringify(card));
        navigate(`/Card`);
    };

    return (
        <div className="search-container">
            <div className="right-nav">
                <button className="navigate-button" onClick={() => navigate('/sim')}>
                    Go to Simulate Pack
                </button>
            </div>
            <h2>MTG Card Search</h2>
            <form onSubmit={handleSearchSubmit} className="search-form">
                <select value={searchType} onChange={(e) => setSearchType(e.target.value)}>
                    <option value="name">Search by Name</option>
                    <option value="type">Search by Creature Type</option>
                    <option value="description">Search by Card Type</option>
                </select>
                <input
                    type="text"
                    placeholder="Search..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                />
                <button type="submit" disabled={loadingCards}>
                    {loadingCards ? 'Searching...' : 'Search'}
                </button>
            </form>
            {loadingCards && <div className="loading-spinner"></div>}
            {error && <p className="error-message">Error: {error}</p>}
            <div className="search-results">
                {cards.length === 0 ? (
                    <p>No cards to display yet...</p>
                ) : (
                    <div className="card-grid">
                        {cards.map((card) => (
                            <div key={card.id} className="card-item" onClick={() => handleCardClick(card)}>
                                <img src={card.image_uris?.small || card.image_uris?.normal || 'placeholder.jpg'} alt={card.name} />
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );

}

export default ScryfallSearchComponent;