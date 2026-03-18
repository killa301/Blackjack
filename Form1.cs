using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Blackjack
{
    public partial class Form1 : Form
    {
        private Deck deck;
        private Hand playerHand, dealerHand;
        private int playerMoney = 1000;
        private int currentBet = 0;

        private PictureBox[] playerCardBoxes = new PictureBox[10];
        private PictureBox[] dealerCardBoxes = new PictureBox[10];
        private Label lblPlayerScore, lblDealerScore, lblMoney, lblResult, lblBet;
        private Button btnHit, btnStand, btnDeal, btnPlusBet, btnMinusBet;

        // Панели
        private Panel panelMenu, panelGame;

        public Form1()
        {
            InitializeComponent();
            CreateMainMenu();
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private void CreateMainMenu()
        {
            this.Text = "Блэкджек";
            this.Size = new Size(900, 650);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(0, 80, 0); // тёмно-зелёный стол

            panelMenu = new Panel { Dock = DockStyle.Fill };

            Label lblTitle = new Label
            {
                Text = "БЛЭКДЖЕК",
                Font = new Font("Segoe UI", 48F, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = true,
                Top = 120,
                Left = (this.ClientSize.Width - 400) / 2
            };

            Label lblAuthor = new Label
            {
                Text = "Классический блэкджек на C# WinForms",
                Font = new Font("Segoe UI", 14F),
                ForeColor = Color.White,
                AutoSize = true,
                Top = 220,
                Left = (this.ClientSize.Width - 400) / 2 + 50
            };

            Button btnStart = new Button
            {
                Text = "ИГРАТЬ",
                Size = new Size(200, 70),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                BackColor = Color.Gold,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Top = 320,
                Left = (this.ClientSize.Width - 200) / 2
            };
            btnStart.Click += (s, e) => StartGame();

            Button btnExit = new Button
            {
                Text = "ВЫХОД",
                Size = new Size(200, 70),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Top = 410,
                Left = (this.ClientSize.Width - 200) / 2
            };
            btnExit.Click += (s, e) => Application.Exit();

            panelMenu.Controls.AddRange(new Control[] { lblTitle, lblAuthor, btnStart, btnExit });
            this.Controls.Add(panelMenu);
        }

        private void StartGame()
        {
            panelMenu.Visible = false;
            panelMenu.Dispose();

            panelGame = new Panel { Dock = DockStyle.Fill };
            this.Controls.Add(panelGame);

            CreateGameTable();
            NewGame();
        }

        private void CreateGameTable()
        {
            // Фон стола
            panelGame.BackgroundImage = CreateTableBackground();
            panelGame.BackgroundImageLayout = ImageLayout.Stretch;

            // Деньги и ставка
            lblMoney = CreateLabel("Деньги: $1000", 20, 10, 20, Color.Gold);
            lblBet = CreateLabel("Ставка: $0", 20, 50, 18, Color.White);

            btnPlusBet = new Button { Text = "+50", Size = new Size(60, 40), Location = new Point(250, 45) };
            btnMinusBet = new Button { Text = "-50", Size = new Size(60, 40), Location = new Point(320, 45) };
            btnDeal = new Button { Text = "РАЗДАТЬ", Size = new Size(120, 50), Location = new Point(700, 40), Enabled = false, BackColor = Color.Gold, Font = new Font("Arial", 12F, FontStyle.Bold) };

            btnPlusBet.Click += (s, e) => ChangeBet(50);
            btnMinusBet.Click += (s, e) => ChangeBet(-50);
            btnDeal.Click += (s, e) => Deal();

            panelGame.Controls.AddRange(new Control[] { lblMoney, lblBet, btnPlusBet, btnMinusBet, btnDeal });

            // Карты дилера
            Label lblDealer = CreateLabel("ДИЛЕР", 20, 120, 20, Color.White);
            lblDealerScore = CreateLabel("", 400, 160, 18, Color.Yellow);
            panelGame.Controls.AddRange(new Control[] { lblDealer, lblDealerScore });

            for (int i = 0; i < 10; i++)
            {
                dealerCardBoxes[i] = new PictureBox
                {
                    Size = new Size(90, 130),
                    Location = new Point(150 + i * 95, 140),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BorderStyle = BorderStyle.FixedSingle,
                    Visible = false
                };
                panelGame.Controls.Add(dealerCardBoxes[i]);
            }

            // Карты игрока
            Label lblPlayer = CreateLabel("ИГРОК", 20, 320, 20, Color.White);
            lblPlayerScore = CreateLabel("", 400, 360, 18, Color.Yellow);
            panelGame.Controls.AddRange(new Control[] { lblPlayer, lblPlayerScore });

            for (int i = 0; i < 10; i++)
            {
                playerCardBoxes[i] = new PictureBox
                {
                    Size = new Size(90, 130),
                    Location = new Point(150 + i * 95, 340),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BorderStyle = BorderStyle.FixedSingle,
                    Visible = false
                };
                panelGame.Controls.Add(playerCardBoxes[i]);
            }

            // Кнопки действий
            btnHit = CreateButton("ВЗЯТЬ", 250, 520, () => Hit());
            btnStand = CreateButton("ХВАТИТ", 450, 520, () => Stand());
            btnHit.Enabled = btnStand.Enabled = false;

            lblResult = CreateLabel("", 300, 480, 24, Color.Gold);
            lblResult.Font = new Font("Arial", 24F, FontStyle.Bold);

            Button btnNewGame = CreateButton("НОВАЯ ИГРА", 650, 520, () => NewGame());
            Button btnBackToMenu = CreateButton("В МЕНЮ", 50, 520, () =>
            {
                panelGame.Controls.Clear();
                panelGame.Dispose();
                CreateMainMenu();
            });

            panelGame.Controls.AddRange(new Control[] { btnHit, btnStand, btnNewGame, btnBackToMenu, lblResult });
        }

        private Label CreateLabel(string text, int x, int y, float fontSize, Color color)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Font = new Font("Segoe UI", fontSize, FontStyle.Bold),
                ForeColor = color,
                AutoSize = true,
                BackColor = Color.Transparent
            };
        }

        private Button CreateButton(string text, int x, int y, Action click)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(150, 60),
                Location = new Point(x, y),
                Font = new Font("Arial", 14F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btn.Click += (s, e) => click();
            return btn;
        }

        private Image CreateTableBackground()
        {
            Bitmap bmp = new Bitmap(1, 1);
            using (Graphics g = Graphics.FromImage(bmp))
                g.Clear(Color.FromArgb(0, 100, 0));
            return bmp;
        }

        private void ChangeBet(int amount)
        {
            int newBet = currentBet + amount;
            if (newBet >= 50 && newBet <= playerMoney && newBet <= 500)
            {
                currentBet = newBet;
                lblBet.Text = $"Ставка: ${currentBet}";
                btnDeal.Enabled = true;
            }
        }

        private void NewGame()
        {
            lblResult.Text = "";
            foreach (var pb in playerCardBoxes) pb.Visible = false;
            foreach (var pb in dealerCardBoxes) pb.Visible = false;
            btnHit.Enabled = btnStand.Enabled = false;
            btnDeal.Enabled = currentBet >= 50;
            currentBet = 0;
            lblBet.Text = "Ставка: $0";
            lblPlayerScore.Text = "";
            lblDealerScore.Text = "";
        }

        private void Deal()
        {
            deck = new Deck();
            playerHand = new Hand();
            dealerHand = new Hand();

            playerHand.AddCard(deck.Draw());
            playerHand.AddCard(deck.Draw());
            dealerHand.AddCard(deck.Draw());
            dealerHand.AddCard(deck.Draw());

            ShowPlayerCards();
            ShowDealerCards(true); // первая карта закрыта

            UpdateScores();
            btnHit.Enabled = btnStand.Enabled = true;
            btnDeal.Enabled = btnPlusBet.Enabled = btnMinusBet.Enabled = false;

            if (playerHand.IsBlackjack())
            {
                EndGame("БЛЭКДЖЕК! Вы выиграли ×1.5!");
            }
        }

        private void Hit()
        {
            playerHand.AddCard(deck.Draw());
            ShowPlayerCards();
            UpdateScores();

            if (playerHand.Score > 21)
            {
                EndGame("Перебор! Вы проиграли.");
            }
        }

        private void Stand()
        {
            btnHit.Enabled = btnStand.Enabled = false;

            // Открываем вторую карту дилера
            ShowDealerCards(false);

            while (dealerHand.Score < 17)
            {
                dealerHand.AddCard(deck.Draw());
                ShowDealerCards(false);
                Application.DoEvents();
                System.Threading.Thread.Sleep(600);
            }

            UpdateScores();
            DetermineWinner();
        }

        private void DetermineWinner()
        {
            if (dealerHand.Score > 21)
                EndGame("Дилер перебрал! Вы выиграли!");
            else if (dealerHand.Score > playerHand.Score)
                EndGame("Дилер выиграл.");
            else if (dealerHand.Score < playerHand.Score)
                EndGame("Вы выиграли!");
            else
                EndGame("Ничья — ставка возвращена.");
        }

        private void EndGame(string message)
        {
            lblResult.Text = message;

            if (message.Contains("БЛЭКДЖЕК"))
                playerMoney += (int)(currentBet * 1.5);
            else if (message.Contains("выиграли") && !message.Contains("Дилер"))
                playerMoney += currentBet;
            else if (message.Contains("Ничья"))
                playerMoney += 0; // ставка возвращается
            else
                playerMoney -= currentBet;

            lblMoney.Text = $"Деньги: ${playerMoney}";
            btnHit.Enabled = btnStand.Enabled = false;
            btnDeal.Enabled = btnPlusBet.Enabled = btnMinusBet.Enabled = playerMoney >= 50;
        }

        private void ShowPlayerCards()
        {
            for (int i = 0; i < playerHand.Cards.Count; i++)
            {
                playerCardBoxes[i].Image = GetCardImage(playerHand.Cards[i]);
                playerCardBoxes[i].Visible = true;
            }
        }

        private void ShowDealerCards(bool hideSecond)
        {
            for (int i = 0; i < dealerHand.Cards.Count; i++)
            {
                if (hideSecond && i == 1)
                    dealerCardBoxes[i].Image = GetBackImage();
                else
                    dealerCardBoxes[i].Image = GetCardImage(dealerHand.Cards[i]);
                dealerCardBoxes[i].Visible = true;
            }
        }

        private void UpdateScores()
        {
            lblPlayerScore.Text = $"Очки: {playerHand.Score}";
            bool hideSecondCard = dealerHand.Cards.Count >= 2 && btnHit.Enabled;
            lblDealerScore.Text = hideSecondCard ? "Очки: ?" : $"Очки: {dealerHand.Score}";
            if (!hideSecondCard)
                lblDealerScore.Text = $"Очки: {dealerHand.Score}";
        }

        private Image GetCardImage(Card card)
        {
            string suit = card.Suit switch
            {
                Suit.Clubs => "C",
                Suit.Diamonds => "D",
                Suit.Hearts => "H",
                Suit.Spades => "S",
                _ => ""
            };

            string rank = card.Rank switch
            {
                Rank.Ace => "A",
                Rank.Jack => "J",
                Rank.Queen => "Q",
                Rank.King => "K",
                _ => ((int)card.Rank).ToString()
            };

            string filename = $"{suit}{rank}.png"; // например: SA.png, H10.png, CQ.png
            string path = Path.Combine(Application.StartupPath, "Resources", filename);

            if (File.Exists(path))
                return Image.FromFile(path);

            // Если картинки нет — рисуем заглушку
            return CreatePlaceholderCard($"{rank}\n{suit}");
        }

        private Image GetBackImage()
        {
            string path = Path.Combine(Application.StartupPath, "Resources", "back.png");
            if (File.Exists(path))
                return Image.FromFile(path);

            return CreatePlaceholderCard("?");
        }

        private Image CreatePlaceholderCard(string text)
        {
            Bitmap bmp = new Bitmap(90, 130);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Navy);
                g.DrawRectangle(Pens.White, 0, 0, 89, 129);
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(text, new Font("Arial", 20F, FontStyle.Bold), Brushes.White, new RectangleF(0, 0, 90, 130), sf);
            }
            return bmp;
        }
    }

    // ==================== Вспомогательные классы ====================

    public class Deck
    {
        private List<Card> cards = new();
        private Random rnd = new Random();

        public Deck()
        {
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                    cards.Add(new Card(suit, rank));
            Shuffle();
        }

        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                var temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }

        public Card Draw()
        {
            var card = cards[0];
            cards.RemoveAt(0);
            return card;
        }
    }

    public class Hand
    {
        public List<Card> Cards { get; } = new();
        public int Score
        {
            get
            {
                int score = 0;
                int aces = 0;
                foreach (var c in Cards)
                {
                    if (c.Rank == Rank.Ace) aces++;
                    else if (c.Rank > Rank.Ten) score += 10;
                    else score += (int)c.Rank;
                }
                while (aces > 0 && score + 11 <= 21)
                {
                    score += 11;
                    aces--;
                }
                score += aces;
                return score;
            }
        }

        public void AddCard(Card card) => Cards.Add(card);

        public bool IsBlackjack() => Cards.Count == 2 && Score == 21;
    }

    public class Card
    {
        public Suit Suit { get; }
        public Rank Rank { get; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }
    }

    public enum Suit { Clubs, Diamonds, Hearts, Spades }
    public enum Rank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }
}