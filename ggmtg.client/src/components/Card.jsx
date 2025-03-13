import { useState, useEffect } from 'react';
import './Card.css';

function CardDetailsPage() {
    const [card, setCard] = useState(null);
    const [expectedValue, setExpectedValue] = useState(null);

    useEffect(() => {
        const storedCard = localStorage.getItem('card');
        if (storedCard) {
            const parsedCard = JSON.parse(storedCard);
            setCard(parsedCard);
            calculateExpectedValue(parsedCard);
        }
    }, []);

    const calculateExpectedValue = (card) => {
        // Assume pull rate data (modify based on real-world set details)
        const pullRates = {
            "common": 0.681,
            "uncommon": 0.324,
            "rare": 0.0986,
            "mythic": 0.0123
        };

        let rarity = card.rarity; // Expected values are based on rarity probabilities
        let price = parseFloat(card.prices?.usd) || 0;
        let probability = pullRates[rarity] || 0;

        let ev = price * probability;
        setExpectedValue(ev > 0 ? `$${ev.toFixed(2)}` : "N/A");
    };

    if (!card) {
        return <p className="error-message">Card details not found.</p>;
    }

    return (
        <div className="card-details-container">
            <div className="card-content">
                <img src={card.image_uris?.normal || 'placeholder.jpg'} alt={card.name} className="card-image" />
                <div className="card-info">
                    <p><strong>Name:</strong> {card.name}</p>
                    <p><strong>Type:</strong> {card.type_line}</p>
                    <p><strong>Description:</strong></p>
                    <p>{card.oracle_text?.split('\n').map((line, index) => (<span key={index}>{line}<br /></span>))}</p>
                    {card.power && <p><strong>Power/Toughness:</strong> {card.power}/{card.toughness}</p>}
                    {card.set_name && <p><strong>Set:</strong> {card.set_name}</p>}
                    <p><strong>Price (USD):</strong> {card.prices?.usd ? `$${card.prices.usd}` : "N/A"}</p>
                    <p><strong>Expected Value:</strong> {`${expectedValue}`}</p>
                </div>
            </div>
        </div>
    );
}

export default CardDetailsPage;
