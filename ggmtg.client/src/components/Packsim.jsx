import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './Packsim.css';

function PackSimulator() {
    const [packs, setPacks] = useState([]);
    const [selectedPack, setSelectedPack] = useState(
        localStorage.getItem('selectedPack') || ''
    );
    const [openedCards, setOpenedCards] = useState(() => {
        const saved = localStorage.getItem('openedCards');
        return saved ? JSON.parse(saved) : [];
    });
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    // fetch available sets once
    useEffect(() => {
        fetchPacks();
    }, []);

    // persist selectedPack
    useEffect(() => {
        localStorage.setItem('selectedPack', selectedPack);
    }, [selectedPack]);

    // persist openedCards whenever they change
    useEffect(() => {
        localStorage.setItem('openedCards', JSON.stringify(openedCards));
    }, [openedCards]);

    const fetchPacks = async () => {
        try {
            const response = await fetch('https://api.scryfall.com/sets');
            const data = await response.json();
            if (data.object === 'error') throw new Error(data.details);

            const mergedPacks = {};
            data.data.forEach((set) => {
                if (/(Token|Booster)/i.test(set.name)) return;
                const baseName = set.name
                    .replace(/(Commander|Draft|Set|Collector)/i, '')
                    .trim();
                if (!mergedPacks[baseName]) {
                    mergedPacks[baseName] = { ...set, codes: [set.code] };
                } else {
                    mergedPacks[baseName].codes.push(set.code);
                }
            });

            setPacks(Object.values(mergedPacks));
        } catch (error) {
            console.error('Error fetching packs:', error);
        }
    };

    const fetchPackCards = async () => {
        if (!selectedPack) return;
        setLoading(true);

        // clear previous cards in state & storage
        setOpenedCards([]);
        localStorage.removeItem('openedCards');

        try {
            const selectedSet = packs.find((p) => p.name === selectedPack);
            if (!selectedSet) throw new Error('Pack not found');

            let allCards = [];
            for (const code of selectedSet.codes) {
                const res = await fetch(
                    `https://api.scryfall.com/cards/search?q=e:${code}&unique=prints`
                );
                const json = await res.json();
                if (json.object !== 'error' && json.data) {
                    allCards = allCards.concat(json.data);
                }
            }

            if (allCards.length === 0) throw new Error('No cards found');

            const selectedCards = generateBoosterPack(allCards);
            setOpenedCards(selectedCards);
        } catch (error) {
            console.error('Error fetching booster:', error);
        } finally {
            setLoading(false);
        }
    };

    const generateBoosterPack = (cards) => {
        const rarities = {
            common: cards.filter((c) => c.rarity === 'common'),
            uncommon: cards.filter((c) => c.rarity === 'uncommon'),
            rare: cards.filter((c) => c.rarity === 'rare'),
            mythic: cards.filter((c) => c.rarity === 'mythic'),
        };

        const getRandomFromPool = (pool, count) => {
            const selection = new Set();
            while (selection.size < count && pool.length > 0) {
                const idx = Math.floor(Math.random() * pool.length);
                selection.add(pool.splice(idx, 1)[0]);
            }
            return Array.from(selection);
        };

        let pack = [];
        pack.push(...getRandomFromPool(rarities.common, 3));
        pack.push(...getRandomFromPool(rarities.uncommon, 2));
        pack.push(
            Math.random() < 0.125 && rarities.mythic.length
                ? getRandomFromPool(rarities.mythic, 1)[0]
                : getRandomFromPool(rarities.rare, 1)[0]
        );
        pack.push(...getRandomFromPool(cards, 1));

        return pack.slice(0, 7);
    };

    const handleCardClick = (card) => {
        localStorage.setItem('card', JSON.stringify(card));
        navigate('/Card');
    };

    return (
        <div className="pack-simulator">
            <div className="left-nav">
                <button
                    className="navigate-button"
                    onClick={() => navigate('/search')}
                >
                    Back to Search
                </button>
            </div>
            <h2>MTG Pack Simulator</h2>

            <label>Choose a booster pack: </label>
            <select
                value={selectedPack}
                onChange={(e) => setSelectedPack(e.target.value)}
            >
                <option value="">Select a booster</option>
                {packs.map((pack) => (
                    <option key={pack.name} value={pack.name}>
                        {pack.name} Booster
                    </option>
                ))}
            </select>

            <button onClick={fetchPackCards} disabled={loading || !selectedPack}>
                {loading ? 'Opening...' : 'Open Pack'}
            </button>

            <div className="opened-cards">
                {openedCards.map((card, idx) => (
                    <div
                        key={idx}
                        className="card"
                        onClick={() => handleCardClick(card)}
                    >
                        <img
                            src={card.image_uris?.normal || 'placeholder.jpg'}
                            alt={card.name}
                        />
                        <p>
                            {card.name} ({card.rarity})
                        </p>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default PackSimulator;
