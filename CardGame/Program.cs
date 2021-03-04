using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp6
{
    class Program
    {
        static void Main(string[] args)
        {
            Deck d = Deck.CreateDeck("Player 1", new PlayingCard(CardValue.Ace, Suit.Clubs), new PlayingCard(CardValue.Ace, Suit.Hearts));

            Deck d1 = new Deck("Player 2");
            d1.Add(d.Remove());
            d1.Add(d.Remove());

            d.Print(true);
            d1.Print(true);
        }
    }


    public enum CardType
    {
        PlayingCard,
        Joker
    }

    public enum Suit
    {
        Hearts,
        Diamonds,
        Spades,
        Clubs,
    }

    public enum Color
    {
        Red,
        Black
    }

    public enum CardValue
    {
        Ace = 1,
        Two = 2,
        Tree = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }

    public abstract class Card
    {
        public abstract int Point
        {
            get;
        }

        public abstract int Order
        {
            get;
        }

        public CardType CardType { get; protected set; }

        public bool IsJoker
        {
            get => this.CardType == CardType.Joker;
        }

        public bool IsPlayingCard
        {
            get => this.CardType == CardType.PlayingCard;
        }

        public abstract void Print();
    }

    public class Joker : Card
    {
        public override int Point => 100;

        public override int Order => 1000;

        public Joker()
        {
            this.CardType = CardType.Joker;
        }

        public override void Print()
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("JOK");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("]");

        }

        public override string ToString()
        {
            return "JO";
        }
    }

    public class PlayingCard : Card
    {
        public override int Point => Convert.ToInt32(this.Value);
        public CardValue Value { get; private set; }
        public Suit Suit { get; private set; }
        public PlayingCard(CardValue value, Suit suit)
        {
            this.Value = value;
            this.Suit = suit;
        }

        private string GetSuit()
        {
            switch (Suit)
            {
                case Suit.Diamonds:
                    return "?";
                case Suit.Spades:
                    return "?";
                case Suit.Clubs:
                    return "?";
                case Suit.Hearts:
                    return "?";
                default:
                    return "?";
            }
        }

        public Color Color
        {
            get
            {
                if (Suit == Suit.Clubs || Suit == Suit.Spades)
                    return Color.Black;
                else
                    return Color.Red;
            }
        }

        public override int Order => (Convert.ToInt32(Suit) * 100) + Convert.ToInt32(Value);

        private string GetValue()
        {
            string v = "";
            if (Convert.ToInt32(Value) >= 2 && Convert.ToInt32(Value) <= 10)
                v = Convert.ToInt32(Value).ToString();
            else if (Convert.ToInt32(Value) == 11)
                v = "J";
            else if (Convert.ToInt32(Value) == 12)
                v = "Q";
            else if (Convert.ToInt32(Value) == 13)
                v = "K";
            else if (Convert.ToInt32(Value) == 1)
                v = "A";
            return v.PadLeft(2);
        }

        public override void Print()
        {
            Console.Write("[");
            if (Color == Color.Red)
                Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(GetSuit());
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(GetValue());
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("]");
        }

        public override string ToString()
        {
            return GetSuit() + GetValue();
        }
    }

    public class Deck
    {
        public string Name { get; private set; }
        private List<Card> cards = new List<Card>();
        private static Random random = new Random();

        public Card Peek(int index)
        {
            return cards[index];
        }

        public Deck(string name)
        {
            if (name.Length > 10)
                throw new ApplicationException("Name must be <= 10 in length");
            this.Name = name;
        }

        public Deck()
        {
        }

        // Indexer (peek)
        public Card this[int i]
        {
            get => cards[i];
        }

        public static Deck CreateDeck(params Card[] cards)
        {
            return CreateDeck(null, cards);
        }

        public static Deck CreateDeck(string name, params Card[] cards)
        {
            Deck d = new Deck(name);
            d.AddToTop(cards);
            d.Order();
            return d;
        }


        public static Deck CreateDeck(int jokers = 0, string name = null)
        {
            Deck d = new Deck(name);
            for (int suit = 0; suit < 4; suit++)
            {
                for (int value = 2; value <= 14; value++)
                {
                    var c = new PlayingCard((CardValue)Enum.Parse(typeof(CardValue), value.ToString()), (Suit)Enum.Parse(typeof(Suit), suit.ToString()));
                    d.AddToTop(c);
                }
            }
            for (int i = 0; i < jokers; i++)
            {
                d.AddToTop(new Joker());
            }
            d.Order();
            return d;
        }
        public void AddToTop(Card card)
        {
            cards.Add(card);
        }

        public void Add(Card card)
        {
            AddToTop(card);
        }

        public Card Remove()
        {
            Card c = cards.Last();
            cards.Remove(c);
            return c;
        }

        public void AddToTop(Card[] newCards)
        {
            cards.AddRange(newCards);
        }

        public void AddToButtom(Card card)
        {
            cards.Insert(0, card);
        }

        public void Move(Card card, Deck deck)
        {
            cards.Remove(card);
            deck.AddToTop(card);
        }


        public void Print(bool printName = false)
        {
            if (printName)
            {
                Console.Write(Name.PadRight(10));
            }
            if (cards.Count == 0)
            {
                Console.WriteLine("[---]");
            }
            else
            {
                int count = 0;
                foreach (var card in cards)
                {
                    card.Print();
                    if (++count % 13 == 0)
                    {
                        Console.WriteLine();
                        count = 0;
                    }
                }
                Console.WriteLine();
            }
        }

        public void Shuffel(int count = 3)
        {
            for (int i = 0; i < count; i++)
                cards = cards.OrderBy(_ => random.Next(1, 100000)).ToList();

        }

        public void Order()
        {
            cards = cards.OrderBy(c => c.Order).ToList();
        }

    }
}