//import { useEffect, useState } from 'react';
//import './Card.css';

//function CardDetailsPage() {
//    const [card, setCard] = useState(null);

//    useEffect(() => {
//        const storedCard = localStorage.getItem('card');
//        if (storedCard) {
//            setCard(JSON.parse(storedCard));
//        }
//    }, []);

//    if (!card) {
//        return <p className="error-message">Card details not found.</p>;
//    }

//    return (
//        <div className="card-details-container">
//            <div className="card-content">
//                <img src={card.image_uris?.normal || 'placeholder.jpg'} alt={card.name} className="card-image" />
//                <div className="card-info">
//                    <p><strong>Name:</strong> {card.name}</p>
//                    <p><strong>Type:</strong> {card.type_line}</p>
//                    <p><strong>Oracle Text:</strong></p><p>{card.oracle_text.split('\n').map((line, index) => (<span key={index}>{line}<br /></span>))}</p>
//                    {card.power && <p><strong>Power/Toughness:</strong> {card.power}/{card.toughness}</p>}
//                    {card.set_name && <p><strong>Set:</strong> {card.set_name}</p>}
//                    <p>Current Price: </p>
//                    <p>Expected value: </p>
//                </div>
//            </div>
//        </div>
//    );
//}

//export default CardDetailsPage;
import { useEffect, useState } from 'react';
import './Card.css';

function CardDetailsPage() {
    const [card, setCard] = useState(null);

    useEffect(() => {
        const storedCard = localStorage.getItem('card');
        if (storedCard) {
            setCard(JSON.parse(storedCard));
        }
    }, []);

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
                    <p><strong>Oracle Text:</strong></p>
                    <p>{card.oracle_text.split('\n').map((line, index) => (<span key={index}>{line}<br /></span>))}</p>
                    {card.power && <p><strong>Power/Toughness:</strong> {card.power}/{card.toughness}</p>}
                    {card.set_name && <p><strong>Set:</strong> {card.set_name}</p>}
                    {card.prices?.usd && <p><strong>Price (USD):</strong> ${card.prices.usd}</p>}
                    <p><strong>Expected Value:</strong> </p>
                </div>
            </div>
        </div>
    );
}

export default CardDetailsPage;
