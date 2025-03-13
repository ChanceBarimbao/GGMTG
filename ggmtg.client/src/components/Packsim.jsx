import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './Packsim.css';

function PackSimulator() {
    const [packs, setPacks] = useState([]);
    const [selectedSet, setSelectedSet] = useState(localStorage.getItem('selectedSet') || '');
    const [openedCards, setOpenedCards] = useState([]);
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        fetchPacks();
    }, []);

    useEffect(() => {
        localStorage.setItem('selectedSet', selectedSet);
    }, [selectedSet]);

    const fetchPacks = async () => {
        try {
            const response = await fetch('https://api.scryfall.com/sets');
            const data = await response.json();
            if (data.data) {
                setPacks(data.data);
            }
        } catch (error) {
            console.error('Error fetching packs:', error);
        }
    };

    const fetchPackCards = async () => {
        if (!selectedSet) return;
        setLoading(true);
        setOpenedCards([]);

        try {
            const response = await fetch(`https://api.scryfall.com/cards/search?q=set:${selectedSet}`);
            const data = await response.json();
            if (!data.data) throw new Error('No cards found');

            const cards = data.data;
            const selectedCards = getRandomCards(cards, 14); // Open a pack with 14 random cards

            setOpenedCards(selectedCards);
        } catch (error) {
            console.error('Error fetching cards:', error);
        }

        setLoading(false);
    };

    const getRandomCards = (cards, count) => {
        return cards.sort(() => 0.5 - Math.random()).slice(0, count);
    };

    const handleCardClick = (card) => {
        localStorage.setItem('card', JSON.stringify(card));
        navigate(`/Card`);
    };

    return (
        <div className="pack-simulator">
            <button className="navigate-button" onClick={() => navigate('/search')}>Back to Search</button>
            <h2>MTG Pack Simulator</h2>

            <label>Choose a set: </label>
            <select value={selectedSet} onChange={(e) => setSelectedSet(e.target.value)}>
                <option value="">Select a set</option>
                {packs.map((set) => (
                    <option key={set.code} value={set.code}>{set.name}</option>
                ))}
            </select>

            <button onClick={fetchPackCards} disabled={loading || !selectedSet}>
                {loading ? 'Opening...' : 'Open Pack'}
            </button>

            <div className="opened-cards">
                {openedCards.length > 0 && openedCards.map((card, index) => (
                    <div key={index} className="card" onClick={() => handleCardClick(card)}>
                        <img src={card.image_uris?.normal || 'placeholder.jpg'} alt={card.name} />
                        <p>{card.name} ({card.rarity})</p>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default PackSimulator;