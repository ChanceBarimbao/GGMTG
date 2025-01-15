import { useEffect, useState } from 'react';

function CardList() {
    const [cards, setCards] = useState([]);

    useEffect(() => {
        // Fetch from our new endpoint
        fetch('/api/CardScrape')
            .then(response => response.json())
            .then(data => {
                setCards(data);
            })
            .catch(error => console.error('Error fetching cards:', error));
    }, []);

    return (
        <div>
            <h1>Scraped Cards</h1>
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
