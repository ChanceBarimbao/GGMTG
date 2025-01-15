////import React from "react";
//import CardList from "./components/CardList";

//function App() {
//    return (
//        <div>
//            <h1>My Card Scraper</h1>
//            <CardList />
//        </div>
//    );
//}

//export default App;
//import { useEffect, useState } from 'react';

//function ScryfallSetsComponent() {
//    const [sets, setSets] = useState([]);
//    const [cardsJson, setCardsJson] = useState('');

//    // Fetch the sets from your .NET backend, which calls Scryfall
//    useEffect(() => {
//        fetch('/api/CardScrape/sets')
//            .then((res) => res.json())
//            .then((data) => setSets(data))
//            .catch((err) => console.error(err));
//    }, []);

//    const handleGetCards = (searchUri) => {
//        // Your .NET endpoint expects ?uri=...
//        fetch(`/api/CardScrape/cards?uri=${encodeURIComponent(searchUri)}`)
//            .then((res) => res.json())
//            .then((data) => {
//                // data is still JSON. We'll just show it as a string.
//                setCardsJson(JSON.stringify(data, null, 2));
//            })
//            .catch((err) => console.error(err));
//    };

//    return (
//        <div>
//            <h1></h1>
//            <ul>
//                {sets.map((s) => (
//                    <li key={s.Id}>
//                        <strong>{s.Name}</strong> ({s.Code})
//                        &nbsp;
//                        <button onClick={() => handleGetCards(s.SearchUri)}>
//                            Get Cards
//                        </button>
//                    </li>
//                ))}
//            </ul>

//            <h2>Cards JSON:</h2>
//            <pre>{cardsJson}</pre>
//        </div>
//    );
//}

//export default ScryfallSetsComponent;

//import { useEffect, useState } from 'react';

//function ScryfallSetsComponent() {
//    const [sets, setSets] = useState([]);
//    const [cards, setCards] = useState([]);       // <--- an array for the fetched cards
//    const [searchCode, setSearchCode] = useState('');

//    // 1. Load the list of sets on mount
//    useEffect(() => {
//        fetch('/api/CardScrape/sets')
//            .then((res) => res.json())
//            .then((data) => setSets(data))
//            .catch((err) => console.error(err));
//    }, []);

//    // 2. Fetch cards for a given 'searchUri' from Scryfall
//    const handleGetCards = (searchUri) => {
//        //fetch(`https://api.scryfall.com/cards/search?order=set&q=e%3Aneo&unique=prints`)
//        fetch(`https://api.scryfall.com/cards/search?order=set&q=e%3A${searchUri}&unique=prints`)
//            .then((res) => res.json())
//            .then((data) => {
//                // Instead of storing as a string, store the array directly
//                // 'data' should be something like:
//                // [
//                //   { id: ..., name: ..., manaCost: ..., typeLine: ..., oracleText: ... },
//                //   ...
//                // ]
//                //console.writeline(searchUri);
//                //setCards(data);
//                setCards(data.data);
//            })
//            .catch((err) => console.error(err));
//    };

//    // 3. When user submits the search form, build a Scryfall 'search_uri' for that set code
//    const handleSearchSubmit = (event) => {
//        event.preventDefault();
//        if (!searchCode) {
//            alert('Please enter a set code, e.g. "neo"');
//            return;
//        }
//        // Example: "neo" => "https://api.scryfall.com/cards/search?order=set&q=e%3Aneo&unique=prints"
//        //const scryfallUri = `https://api.scryfall.com/cards/search?order=set&q=e%3A${searchCode}&unique=prints`;
//        const scryfallUri = `https://api.scryfall.com/cards/search?order=set&q=e%3A${searchCode}&unique=prints`;
//        //const scryfallUri =  "https://api.scryfall.com/cards/search?order=set&q=e%3Aneo&unique=prints";
//        handleGetCards(scryfallUri);
//    };

//    return (
//        <div>
//            <h1>MTG Set Search</h1>

//            {/* Search form for a single set code */}
//            <form onSubmit={handleSearchSubmit} style={{ marginBottom: '1rem' }}>
//                <input
//                    type="text"
//                    placeholder='e.g. "neo"'
//                    value={searchCode}
//                    onChange={(e) => setSearchCode(e.target.value.toLowerCase())}
//                    style={{ marginRight: '0.5rem' }}
//                />
//                <button type="submit">Fetch Single Set</button>
//            </form>

//            {/* Display all sets fetched on mount */}
//            <ul>
//                {sets.map((s) => (
//                    <li key={s.Id}>
//                        <strong>{s.Name}</strong> ({s.Code}){' '}
//                        <button onClick={() => handleGetCards(s.SearchUri)}>
//                            Get Cards
//                        </button>
//                    </li>
//                ))}
//            </ul>

//            <h2>Fetched Cards:</h2>
//            {cards.length === 0 ? (
//                <p>No cards to display (try clicking a set or using the search box)</p>
//            ) : (
//                <ul>
//                    {cards.map((card) => (
//                        <li key={card.id}>
//                            <strong>{card.name ?? 'Unknown Name'}</strong>
//                            {card.typeLine && ` | ${card.typeLine}`}
//                            {card.oracleText && ` | ${card.oracleText}`}
//                            {/* If you have manaCost or other fields, you can display them here too */}
//                        </li>
//                    ))}
//                </ul>
//            )}
//        </div>
//    );
//}

//export default ScryfallSetsComponent;
//import React, { useEffect, useState } from 'react';

//function ScryfallSetsComponent() {
//    const [allSets, setAllSets] = useState([]);
//    const [cards, setCards] = useState([]);
//    const [searchCode, setSearchCode] = useState('');
//    const [loadingSets, setLoadingSets] = useState(true);
//    const [error, setError] = useState('');

//    // 1. On mount, fetch https://api.scryfall.com/sets
//    useEffect(() => {
//        fetch('https://api.scryfall.com/sets')
//            .then((res) => res.json())
//            .then((json) => {
//                if (json.data) {
//                    setAllSets(json.data);
//                } else {
//                    setError('Unexpected Scryfall format');
//                }
//            })
//            .catch((err) => setError(String(err)))
//            .finally(() => setLoadingSets(false));
//    }, []);

//    // 2. Build & fetch cards for a given code
//    const handleGetCardsByCode = (code) => {
//        const scryfallUri = `https://api.scryfall.com/cards/search?order=set&q=e%3A${code}&unique=prints`;

//        fetch(scryfallUri)
//            .then((res) => res.json())
//            .then((data) => setCards(data.data || []))
//            .catch((err) => console.error(err));
//    };

//    // 3. Or use the user’s typed `searchCode`
//    const handleSearchSubmit = (e) => {
//        e.preventDefault();
//        if (!searchCode) return alert('Please enter a set code.');
//        handleGetCardsByCode(searchCode);
//    };

//    if (loadingSets) return <p>Loading Scryfall sets...</p>;
//    if (error) return <p>Error: {error}</p>;

//    return (
//        <div>
//            <h1>All Scryfall Sets</h1>

//            {/* Display code → name for all sets */}
//            <ul>
//                {allSets.map((s) => (
//                    <li key={s.id}>
//                        <code>"{s.code}"</code> → {s.name}
//                        <button onClick={() => handleGetCardsByCode(s.code)}>
//                            Show Cards
//                        </button>
//                    </li>
//                ))}
//            </ul>

//            {/* Search form if user wants to type a code */}
//            <form onSubmit={handleSearchSubmit}>
//                <input
//                    type="text"
//                    placeholder='e.g. "neo"'
//                    value={searchCode}
//                    onChange={(e) => setSearchCode(e.target.value.toLowerCase())}
//                />
//                <button type="submit">Search</button>
//            </form>

//            <h2>Fetched Cards:</h2>
//            {cards.length === 0 ? (
//                <p>No cards to display yet...</p>
//            ) : (
//                <ul>
//                    {cards.map((c) => (
//                        <li key={c.id}>
//                            <strong>{c.name}</strong>
//                            {c.type_line ? ` | ${c.type_line}` : ''}
//                            {c.oracle_text ? ` | ${c.oracle_text}` : ''}
//                        </li>
//                    ))}
//                </ul>
//            )}
//        </div>
//    );
//}

//export default ScryfallSetsComponent;
import React, { useEffect, useState } from 'react';

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
                            <code>"{s.code}"</code> → {s.name}
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

