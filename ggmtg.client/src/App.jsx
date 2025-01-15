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
import { useEffect, useState } from 'react';

function ScryfallSetsComponent() {
    const [sets, setSets] = useState([]);
    const [cardsJson, setCardsJson] = useState('');

    // Fetch the sets from your .NET backend, which calls Scryfall
    useEffect(() => {
        fetch('/api/CardScrape/sets')
            .then((res) => res.json())
            .then((data) => setSets(data))
            .catch((err) => console.error(err));
    }, []);

    const handleGetCards = (searchUri) => {
        // Your .NET endpoint expects ?uri=...
        fetch(`/api/CardScrape/cards?uri=${encodeURIComponent(searchUri)}`)
            .then((res) => res.json())
            .then((data) => {
                // data is still JSON. We'll just show it as a string.
                setCardsJson(JSON.stringify(data, null, 2));
            })
            .catch((err) => console.error(err));
    };

    return (
        <div>
            <h1></h1>
            <ul>
                {sets.map((s) => (
                    <li key={s.Id}>
                        <strong>{s.Name}</strong> ({s.Code})
                        &nbsp;
                        <button onClick={() => handleGetCards(s.SearchUri)}>
                            Get Cards
                        </button>
                    </li>
                ))}
            </ul>

            <h2>Cards JSON:</h2>
            <pre>{cardsJson}</pre>
        </div>
    );
}

export default ScryfallSetsComponent;

