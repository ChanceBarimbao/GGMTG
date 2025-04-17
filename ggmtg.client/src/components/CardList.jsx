
import { useEffect, useState } from 'react';

function CardList() {
    const [cards, setCards] = useState([]);
    const [searchInput, setSearchInput] = useState('');

    // On mount, fetch the default endpoint (/api/CardScrape)
    useEffect(() => {
        fetch('/api/CardScrape')
            .then((response) => response.json())
            .then((data) => {
                setCards(data);
            })
            .catch((error) => console.error('Error fetching cards:', error));
    }, []);

    // Handle the search form submission
    const handleSearchSubmit = (e) => {
        e.preventDefault();

        // Call your new endpoint: /api/CardScrape/cards?setName=theInput
        // or however you’ve defined it in your controller
        fetch(`/api/CardScrape/cards?setName=${encodeURIComponent(searchInput)}`)
            .then((response) => response.json())
            .then((data) => {
                setCards(data);
            })
            .catch((error) => console.error('Error searching cards:', error));
    };

    return (
        <div>
            <h1>Scraped Cards</h1>

            {/* Search Form */}
            <form onSubmit={handleSearchSubmit} style={{ marginBottom: '1rem' }}>
                <input
                    type="text"
                    value={searchInput}
                    placeholder="Type a set name or code"
                    onChange={(e) => setSearchInput(e.target.value)}
                    style={{ marginRight: '0.5rem' }}
                />
                <button type="submit">Search</button>
            </form>

            {/* Display Cards */}
            <ul>
                {cards.map((card, index) => (
                    <li key={index}>
                        <strong>{card.name}</strong>
                        {' - '}
                        Set: {card.setName} | Rarity: {card.rarity} | Price: {card.price}
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default CardList;
