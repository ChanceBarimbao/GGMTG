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

import { useEffect, useState } from 'react';

function ScryfallSetsComponent() {
    const [sets, setSets] = useState([]);
    const [cards, setCards] = useState([]);       // <--- an array for the fetched cards
    const [searchCode, setSearchCode] = useState('');

    // 1. Load the list of sets on mount
    useEffect(() => {
        fetch('/api/CardScrape/sets')
            .then((res) => res.json())
            .then((data) => setSets(data))
            .catch((err) => console.error(err));
    }, []);

    // 2. Fetch cards for a given 'searchUri' from Scryfall
    const handleGetCards = (searchUri) => {
        fetch(`/api/CardScrape/cards?uri=${encodeURIComponent(searchUri)}`)
            .then((res) => res.json())
            .then((data) => {
                // Instead of storing as a string, store the array directly
                // 'data' should be something like:
                // [
                //   { id: ..., name: ..., manaCost: ..., typeLine: ..., oracleText: ... },
                //   ...
                // ]
                setCards(data);
            })
            .catch((err) => console.error(err));
    };

    // 3. When user submits the search form, build a Scryfall 'search_uri' for that set code
    const handleSearchSubmit = (event) => {
        event.preventDefault();
        if (!searchCode) {
            alert('Please enter a set code, e.g. "neo"');
            return;
        }
        // Example: "neo" => "https://api.scryfall.com/cards/search?order=set&q=e%3Aneo&unique=prints"
        //const scryfallUri = `https://api.scryfall.com/cards/search?order=set&q=e%3A${searchCode}&unique=prints`;
        const scryfallUri =  "https://api.scryfall.com/cards/search?order=set&q=e%3Aneo&unique=prints";
        handleGetCards(scryfallUri);
    };

    return (
        <div>
            <h1>MTG Set Search</h1>

            {/* Search form for a single set code */}
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

            {/* Display all sets fetched on mount */}
            <ul>
                {sets.map((s) => (
                    <li key={s.Id}>
                        <strong>{s.Name}</strong> ({s.Code}){' '}
                        <button onClick={() => handleGetCards(s.SearchUri)}>
                            Get Cards
                        </button>
                    </li>
                ))}
            </ul>

            <h2>Fetched Cards:</h2>
            {cards.length === 0 ? (
                <p>No cards to display (try clicking a set or using the search box)</p>
            ) : (
                <ul>
                    {cards.map((card) => (
                        <li key={card.id}>
                            <strong>{card.name ?? 'Unknown Name'}</strong>
                            {card.typeLine && ` | ${card.typeLine}`}
                            {card.oracleText && ` | ${card.oracleText}`}
                            {/* If you have manaCost or other fields, you can display them here too */}
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}

export default ScryfallSetsComponent;
