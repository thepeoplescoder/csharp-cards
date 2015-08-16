using System;
using System.Collections.Generic;

namespace Cards
{
    // So I'm not constantly recreating Random objects.
    internal class __Rand
    {
        public static Random r = new Random();
        public static int Next(int max)
        {
            return r.Next(max);
        }
    }

    // This interface is to be used with a class
    // that has the ability to store cards.
    public interface ICanStoreCards
    {
        void GainCard(Card card);   // To be called when a class gains a card
        void LoseCard(Card card);   // To be called when a class loses a card
    }

    // A simple card class.
    // I put the enumerations inside of the class
    // to avoid name conflicts.
    public class Card
    {
        // JokerType
        public enum JokerType
        {
            Little, Big
        }

        // SuitType
        public enum SuitType
        {
            Hearts, Diamonds, Spades, Clubs,
            Joker, BigJoker
        }

        // SuitColorType
        public enum SuitColorType
        {
            None, Red, Black
        }

        // Spot values
        public enum SpotType
        {
            Two = 2, Three, Four,
            Five, Six, Seven,
            Eight, Nine, Ten,

            Jack, Queen, King,

            Ace, Joker, BigJoker
        }

        // Private instance variables
        private SuitType m_suit;
        private SpotType m_spot;
        private object m_owner;     // Where is the card currently?

        //
        // Static helper methods
        //

        // IsJokerSuit ////////////////////////////////////
        public static bool IsJokerSuit(SuitType suit)
        {
            return suit == SuitType.BigJoker || suit == SuitType.Joker;
        }

        // IsJokerSpot ////////////////////////////////////
        public static bool IsJokerSpot(SpotType spot)
        {
            return spot == SpotType.BigJoker || spot == SpotType.Joker;
        }

        //
        // Properties (for encapsulation)
        //

        // The suit
        public SuitType Suit
        {
            private set { m_suit = value; }
            get { return m_suit; }
        }

        // The value of the card
        public SpotType Spot
        {
            private set { m_spot = value; }
            get { return m_spot; }
        }

        // Does this card have an owner?
        public bool HasOwner
        {
            get { return Owner != null; }
        }

        // Is this card the big joker?
        public bool IsBigJoker
        {
            get { return Suit == SuitType.BigJoker && Spot == SpotType.BigJoker; }
        }

        // Is this card the little joker?
        public bool IsLittleJoker
        {
            get { return Suit == SuitType.Joker && Spot == SpotType.Joker; }
        }

        // Is this card a joker?
        public bool IsJoker
        {
            get { return IsBigJoker || IsLittleJoker; }
        }

        // Is this card a diamond?
        public bool IsDiamond
        {
            get { return Suit == SuitType.Diamonds; }
        }

        // Is this card a heart?
        public bool IsHeart
        {
            get { return Suit == SuitType.Hearts; }
        }

        // Is this card a club?
        public bool IsClub
        {
            get { return Suit == SuitType.Clubs; }
        }

        // Is this card a spade?
        public bool IsSpade
        {
            get { return Suit == SuitType.Spades; }
        }

        // Is this card red?
        public bool IsRed
        {
            get { return IsHeart || IsDiamond; }
        }

        // Is this card black?
        public bool IsBlack
        {
            get { return IsClub || IsSpade; }
        }

        // The suit color
        public SuitColorType SuitColor
        {
            get
            {
                if (IsBlack)
                {
                    return SuitColorType.Black;
                }
                else if (IsRed)
                {
                    return SuitColorType.Red;
                }
                else
                {
                    return SuitColorType.None;
                }
            }
        }

        // Is this card a face card?
        public bool IsFaceCard
        {
            get
            {
                return Spot == SpotType.Jack || Spot == SpotType.Queen ||
                       Spot == SpotType.King;
            }
        }

        // Is this card a spot card?
        public bool IsSpotCard
        {
            get
            {
                return !IsFaceCard && !IsJoker;
            }
        }

        // Who currently owns the card
        // i.e. Does a player have it in his/her hand,
        // is it currently in the deck, is it in
        // a discard pile, etc.
        public object Owner
        {
            // Changes the owner, calling necessary methods
            set
            {
                ICanStoreCards i;

                // Call the lose card method of the current owner
                if (HasOwner)
                {
                    i = m_owner as ICanStoreCards;
                    if (i != null)
                    {
                        i.LoseCard(this);
                    }
                }

                // Okay, NOW we can change the owner.
                m_owner = value;

                // Call the gain card method of the new owner
                if (HasOwner)
                {
                    i = m_owner as ICanStoreCards;
                    if (i != null)
                    {
                        i.GainCard(this);
                    }
                }
            }
            get { return m_owner; }
        }

        // The name of the card
        public string Name
        {
            get
            {
                if (IsBigJoker)
                {
                    return "Big Joker";
                }
                else if (IsLittleJoker)
                {
                    return "Little Joker";
                }
                else
                {
                    return Enum.GetName(typeof(SpotType), Spot) + " of " + Enum.GetName(typeof(SuitType), Suit);
                }
            }
        }

        //
        // Constructors
        //

        // A card with an owner
        public Card(SuitType suit, SpotType spot, object owner)
        {
            // Force a joker if one is requested.
            if (suit == SuitType.Joker)                 // Little
            {
                spot = SpotType.Joker;
            }
            else if (spot == SpotType.Joker)
            {
                suit = SuitType.Joker;
            }

            else if (suit == SuitType.BigJoker)         // Big
            {
                spot = SpotType.BigJoker;
            }
            else if (spot == SpotType.BigJoker)
            {
                suit = SuitType.BigJoker;
            }

            // Initalize the fields
            Suit = suit;
            Spot = spot;
            Owner = owner;
        }

        // A card with no owner
        public Card(SuitType suit, SpotType spot) :
            this(suit, spot, null)
        { }

        // A static method to make a joker
        public static Card NewJoker(JokerType jokerType, object owner)
        {
            SuitType s = (jokerType == JokerType.Big) ? SuitType.BigJoker : SuitType.Joker;

            // Allow the constructor to force the proper type.
            return new Card(s, SpotType.Two, owner);
        }
        public static Card NewJoker(JokerType jokerType)
        {
            return Card.NewJoker(jokerType, null);
        }

        // Equals /////////////////////////////////////////
        public bool Equals(Card card)
        {
            // If not a valid card, it's not equal.
            if (card == null)
            {
                return false;
            }

            // I am only concerned with the actual value of the card,
            // and not the owners.
            return this.Suit == card.Suit && this.Spot == card.Spot;
        }
    }

    // For anything capable of storing cards.
    public class CardCollection : ICanStoreCards
    {
        private List<Card> m_cards;
        private readonly int m_MaxCards;
        private bool m_allowDuplicates;

        //
        // Constructors
        //

        // For a collection of any size.
        public CardCollection(int numCards, bool allowDuplicates)
        {
            m_cards = new List<Card>(numCards);
            m_MaxCards = numCards;
            m_allowDuplicates = allowDuplicates;
        }

        // By default, the collection should not allow duplicates.
        public CardCollection(int numCards) :
            this(numCards, false)
        { }

        // Default constructor
        public CardCollection() :
            this(54)
        { }

        //
        // Properties
        //

        // Access to the list
        protected List<Card> _Cards
        {
            set { m_cards = value; }
            get { return m_cards; }
        }

        // Get the cards
        public IList<Card> Cards
        {
            get { return _Cards.AsReadOnly(); }
        }

        // Get a specific card
        public Card this[int index]
        {
            get
            {
                return _Cards[index];
            }
        }

        // Number of cards in the collection
        public int CardCount
        {
            get { return _Cards.Count; }
        }

        // Maximum number of cards this collection can contain
        public int MaxCards
        {
            get { return m_MaxCards; }
        }

        // AllowDuplicates ////////////////////////////////
        public bool AllowDuplicates
        {
            get { return m_allowDuplicates; }
        }

        //
        // Methods
        //

        // addCard ////////////////////////////////////////
        private void addCard(Card card)
        { // Adds a card to the deck.  For encapsulation.
            if (CardCount < MaxCards)
            {
                _Cards.Add(card);
            }
        }

        // removeCard /////////////////////////////////////
        private void removeCard(Card card)
        { // Removes a card from the deck.  For encapsulation.
            _Cards.Remove(card);
        }

        // AddCard ////////////////////////////////////////
        public bool AddCard(Card card)
        { // Adds a card to this collection.
            int cards = CardCount;
            card.Owner = this;              // Simply change the owner.  The property will call the appropriate interface methods.
            return cards != CardCount;      // On a successful add, this quantity will change.
        }

        // SendReceivedCardTo /////////////////////////////
        public bool SendReceivedCardTo(int index)
        { /* The most recently added card will be sent to the "bottom."
           * This method will place that card elsewhere.
           * 
           * This method returns false if it didn't do anything.
           */
            int lastIndex = CardCount - 1;
            if (index < lastIndex)
            {
                Card card = this[lastIndex];
                _Cards.RemoveAt(lastIndex);
                _Cards.Insert(index, card);
                return true;
            }
            return false;
        }

        //
        // Interface methods
        //

        // GainCard
        public virtual void GainCard(Card card)
        {
            // If we are not allowing duplicates, then don't let any through.
            if (!AllowDuplicates)
            {
                foreach (Card c in Cards)
                {
                    if (card.Equals(c))
                    {
                        return;
                    }
                }
            }

            // Add the card.
            addCard(card);
        }

        // LoseCard
        public virtual void LoseCard(Card card)
        { // The method that rids the collection of this card.
            removeCard(card);
        }
    }

    // The Deck class
    // Index 0 will be the topmost card.
    public class Deck : CardCollection
    {
        // Deck types
        public enum DeckType
        {
            Standard52, Standard54
        }

        //
        // Instance helper methods
        //

        // makeDeck52 /////////////////////////////////////
        private void makeDeck52()
        {
            foreach (Card.SuitType suit in Enum.GetValues(typeof(Card.SuitType)))
            {
                if (!Card.IsJokerSuit(suit))
                {
                    foreach (Card.SpotType spot in Enum.GetValues(typeof(Card.SpotType)))
                    {
                        if (!Card.IsJokerSpot(spot))
                        {
                            new Card(suit, spot, this);
                        }
                    }
                }
            }
        }

        // makeDeck /////////////////////////////////////
        private Deck makeDeck(bool is54)
        {
            this.makeDeck52();
            if (is54)
            {
                Card.NewJoker(Card.JokerType.Big, this);
                Card.NewJoker(Card.JokerType.Little, this);
            }
            return this;
        }

        //
        // Constructors
        //

        public Deck(int numCards, bool allowDuplicates) :
            base(numCards, allowDuplicates)
        { }

        public Deck(DeckType dt) :
            this(dt == DeckType.Standard54 ? 54 : 52, false)
        { }

        //
        // Static methods to make standard decks
        //

        // MakeStandard ///////////////////////////////////
        public static Deck MakeStandard(DeckType deckType)
        {
            return new Deck(deckType).makeDeck(deckType == DeckType.Standard54);
        }

        //
        // Deck manipulation
        //

        // SendTopCardTo //////////////////////////////////
        public bool SendTopCardTo(object newOwner)
        { // Sends a card somewhere else (equivalent to dealing a card)
            if (CardCount > 0)
            {
                this[0].Owner = newOwner;
                return true;
            }
            return false;
        }

        // SendBottomCardTo ///////////////////////////////
        public bool SendBottomCardTo(object newOwner)
        { // Equivalent to dealing from the bottom.
            if (CardCount > 0)
            {
                this[CardCount - 1].Owner = newOwner;
                return true;
            }
            return false;
        }

        // CutFromTop /////////////////////////////////////
        public bool CutFromTop(int numCards)
        { // Cuts a specific number of cards from the top to the bottom.
            if (numCards < CardCount)
            {
                List<Card> topCards = _Cards.GetRange(0, numCards);
                _Cards.RemoveRange(0, numCards);
                _Cards.AddRange(topCards);
                return true;
            }
            return false;
        }

        // IdealShuffle ///////////////////////////////////
        public void IdealShuffle()
        { // Performs what would be considered an "ideal" shuffle by human hands.
            int halfCards = CardCount >> 1;
            int topIndex, bottomIndex, topMax, bottomMax;
            int deckIndex;
            bool top;
            List<Card> topHalf = _Cards.GetRange(0, halfCards);

            topIndex = 0;
            topMax = halfCards;
            bottomIndex = halfCards;
            bottomMax = CardCount;
            deckIndex = 0;
            top = true;

            while (topIndex < topMax || bottomIndex < bottomMax)
            {
                if (top && topIndex < topMax)
                {
                    _Cards[deckIndex++] = topHalf[topIndex++];
                }
                else if (bottomIndex < bottomMax)
                {
                    _Cards[deckIndex++] = _Cards[bottomIndex++];
                }
                top = !top;
            }
        }

        // IdealShuffle ///////////////////////////////////
        public void IdealShuffle(int num)
        {
            for (int i = 0; i < num; i++)
            {
                IdealShuffle();
            }
        }

        // Shuffle ////////////////////////////////////////
        public void Shuffle()
        { // Performs an algorithmic shuffle.
            Card temp;
            int index;
            for (int i = 0; i < CardCount; i++)
            {
                index = __Rand.Next(CardCount);
                temp = _Cards[i];
                _Cards[i] = _Cards[index];
                _Cards[index] = temp;
            }
        }

        // Shuffle ////////////////////////////////////////
        public void Shuffle(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Shuffle();
            }
        }

        /*
         * This was just to test everything to make sure
         * it was working properly..  In a sense.
         */
        public static void Main(string[] args)
        {
            Deck d = MakeStandard(DeckType.Standard54);

            d.Shuffle(10);

            foreach (Card card in d.Cards)
            {
                Console.WriteLine(card.Name);
            }
        }
    }
}