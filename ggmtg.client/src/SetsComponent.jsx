//export default ScryfallSetsComponent;
import { useEffect, useState } from 'react';

function ScryfallSetsComponent() {
    const [allSets, setAllSets] = useState([]);     // all sets from Scryfall
    const [cards, setCards] = useState([]);         // cards from selected set
    const [searchCode, setSearchCode] = useState('');
    const [loadingSets, setLoadingSets] = useState(true);
    const [error, setError] = useState('');

    // 1. On mount, fetch Scryfall /sets
    useEffect(() => {
        fetch('https://api.scryfall.com/sets')
            .then((res) => res.json())
            .then((json) => {
                if (json.data) {
                    setAllSets(json.data);
                } else {
                    setError('Unexpected data from Scryfall /sets');
                }
            })
            .catch((err) => setError(String(err)))
            .finally(() => setLoadingSets(false));
    }, []);

    // 2. Build & fetch cards for a given set code
    const handleGetCardsByCode = (code) => {
        const scryfallUri = `https://api.scryfall.com/cards/search?order=set&q=e%3A${code}&unique=prints`;
        fetch(scryfallUri)
            .then((res) => res.json())
            .then((data) => {
                // data.data should be an array of cards
                setCards(data.data || []);
            })
            .catch((err) => console.error(err));
    };

    // 3. Or let user type a set code
    const handleSearchSubmit = (e) => {
        e.preventDefault();
        if (!searchCode) {
            alert('Please enter a set code.');
            return;
        }
        handleGetCardsByCode(searchCode);
    };

    // If sets are still loading or an error occurred
    if (loadingSets) return <p>Loading sets...</p>;
    if (error) return <p>Error: {error}</p>;

    return (
        <div style={{ display: 'flex', gap: '1rem' }}>
            {/* Left side: SCROLLABLE LIST OF SETS */}
            <div
                style={{
                    width: '250px',
                    maxHeight: '80vh',
                    overflowY: 'auto',
                    border: '1px solid #ccc',
                    padding: '0.5rem',
                }}
            >
                <h3>All Scryfall Sets</h3>
                <ul style={{ listStyleType: 'none', padding: 0, margin: 0 }}>
                    {allSets.map((s) => (
                        <li key={s.id} style={{ marginBottom: '0.5rem' }}>
                            <code>`{s.code}`</code> → {s.name}
                            <button
                                onClick={() => handleGetCardsByCode(s.code)}
                                style={{ marginLeft: '0.5rem' }}
                            >
                                Show Cards
                            </button>
                        </li>
                    ))}
                </ul>
            </div>

            {/* Right side: MAIN CONTENT (search form & card list) */}
            <div style={{ flex: 1 }}>
                <h2>MTG Set Search</h2>

                {/* Search form for code */}
                <form onSubmit={handleSearchSubmit} style={{ marginBottom: '1rem' }}>
                    <input
                        type="text"
                        placeholder='e.g. "neo"'
                        value={searchCode}
                        onChange={(e) => setSearchCode(e.target.value.toLowerCase())}
                        style={{ marginRight: '0.5rem' }}
                    />
                    <button type="submit">Fetch Single Set</button>
                </form>

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
    );
}

export default ScryfallSetsComponent;

