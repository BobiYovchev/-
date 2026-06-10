using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Game game = new Game();
            game.Start();
        }
        public class Game
        {
            private Character _player;
            private List<Monster> _monsters;
            private int _currentMonsterIndex;

            public Game()
            {
                _monsters = new List<Monster>
                {
                new Monster("🐺 Вълк",        50, 10, 2, "Зверове"),
                new Monster("🧟 Зомби",       70, 13, 4, "Нежив"),
                    new Monster("🧙 Тъмен магьосник", 90, 20, 5, "Магия"),
                new Monster("🐉 Дракон",      150, 28, 10, "Легендарен"),
            };
                _currentMonsterIndex = 0;
            }

            public void Start()
            {
                PrintTitle();
                ChooseHero();
                RunGameLoop();
            }

            private void PrintTitle()
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("╔══════════════════════════════════════╗");
                Console.WriteLine("║        ⚔️   RPG  ARENA  ⚔️           ║");
                Console.WriteLine("║   Герои срещу Чудовища - C# ООП      ║");
                Console.WriteLine("╚══════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();
            }

            private void ChooseHero()
            {
                Console.WriteLine("Избери своя герой:\n");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  [1] " + new Warrior("?").GetClassInfo());
                Console.WriteLine("  [2] " + new Mage("?").GetClassInfo());
                Console.WriteLine("  [3] " + new Rogue("?").GetClassInfo());
                Console.ResetColor();
                Console.Write("\nИзбор (1/2/3): ");

                string choice = Console.ReadLine();

                Console.Write("Въведи името на героя си: ");
                string name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name)) name = "Герой";

                // ===== ПОЛИМОРФИЗЪМ =====
                // _player е от тип Character, но сочи към Warrior/Mage/Rogue
                switch (choice)
                {
                    case "1": _player = new Warrior(name); break;
                    case "2": _player = new Mage(name); break;
                    case "3": _player = new Rogue(name); break;
                    default: _player = new Warrior(name); break;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✅ Избра: {_player.GetClassInfo()}");
                Console.ResetColor();
                Pause();
            }

            private void RunGameLoop()
            {
                while (_player.IsAlive && _currentMonsterIndex < _monsters.Count)
                {
                    Monster enemy = _monsters[_currentMonsterIndex];
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n══════ Битка {_currentMonsterIndex + 1} / {_monsters.Count} ══════");
                    Console.WriteLine($"Появява се: {enemy.Name}!");
                    Console.ResetColor();

                    RunBattle(_player, enemy);

                    if (_player.IsAlive)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n🏆 Победи {enemy.Name}!");
                        Console.ResetColor();
                        _currentMonsterIndex++;

                        if (_currentMonsterIndex < _monsters.Count)
                        {
                            Console.Write("Продължи към следващия враг? (Enter)");
                            Console.ReadLine();
                        }
                    }
                }

                PrintResult();
            }

            private void RunBattle(Character hero, Monster enemy)
            {
                Random rng = new Random();
                int round = 1;

                while (hero.IsAlive && enemy.IsAlive)
                {
                    Console.WriteLine($"\n--- Рунд {round} ---");
                    hero.ShowStats();
                    enemy.ShowStats();
                    Console.WriteLine();

                    Console.Write("Действие: [1] Атака  [2] Специален удар  [3] Статистики > ");
                    string action = Console.ReadLine();

                    if (action == "3")
                    {
                        Console.WriteLine($"\n{hero.GetClassInfo()}");
                        continue;
                    }

                    // ===== ПОЛИМОРФИЗЪМ =====
                    // hero.Attack() извиква различна логика според реалния тип (Warrior/Mage/Rogue)
                    int heroDamage = hero.Attack();
                    enemy.TakeDamage(heroDamage);

                    if (!enemy.IsAlive) break;

                    // Ход на врага
                    int enemyDamage = enemy.Attack();
                    hero.TakeDamage(enemyDamage);

                    round++;
                }
            }

            private void PrintResult()
            {
                Console.WriteLine();
                if (_player.IsAlive)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("╔══════════════════════════════════════╗");
                    Console.WriteLine("║   🏆  ПОБЕДА! Победи всички врагове! ║");
                    Console.WriteLine("╚══════════════════════════════════════╝");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("╔══════════════════════════════════════╗");
                    Console.WriteLine("║        💀  ЗАГУБА! Падна в битка...  ║");
                    Console.WriteLine("╚══════════════════════════════════════╝");
                }
                Console.ResetColor();
            }

            private void Pause()
            {
                Console.Write("\nПресни Enter за да продължиш...");
                Console.ReadLine();
            }
        }
    }
    // ===== АБСТРАКЦИЯ =====
    // Абстрактен базов клас - не може да се инстанцира директно
    public abstract class Character
    {
        // ===== ЕНКАПСУЛАЦИЯ =====
        // Полетата са private - достъпни само вътре в класа
        private string _name;
        private int _health;
        private int _maxHealth;
        private int _attackPower;
        private int _defense;

        // Public properties - контролиран достъп до private полетата
        public string Name
        {
            get { return _name; }
            protected set { _name = value; }
        }

        public int Health
        {
            get { return _health; }
            protected set { _health = value < 0 ? 0 : value; }
        }

        public int MaxHealth
        {
            get { return _maxHealth; }
            protected set { _maxHealth = value; }
        }

        public int AttackPower
        {
            get { return _attackPower; }
            protected set { _attackPower = value; }
        }

        public int Defense
        {
            get { return _defense; }
            protected set { _defense = value; }
        }

        public bool IsAlive => Health > 0;

        // Конструктор на базовия клас
        public Character(string name, int health, int attackPower, int defense)
        {
            Name = name;
            MaxHealth = health;
            Health = health;
            AttackPower = attackPower;
            Defense = defense;
        }

        // ===== ПОЛИМОРФИЗЪМ =====
        // Virtual метод - може да се override-не в наследниците
        public virtual int Attack()
        {
            return AttackPower;
        }

        // Abstract метод - ЗАДЪЛЖИТЕЛНО се override-ва от наследниците
        public abstract string GetClassInfo();

        // Обикновен метод - споделен от всички
        public void TakeDamage(int damage)
        {
            int actualDamage = Math.Max(1, damage - Defense);
            Health -= actualDamage;
            Console.WriteLine($"  {Name} получава {actualDamage} щета! (HP: {Health}/{MaxHealth})");
        }

        public void ShowStats()
        {
            Console.WriteLine($"  [{Name}] HP: {Health}/{MaxHealth} | ATK: {AttackPower} | DEF: {Defense}");
        }
    }

    // ===== НАСЛЕДЯВАНЕ =====
    // Warrior наследява Character
    public class Warrior : Character
    {
        private int _rage; // допълнително поле само за Warrior

        public Warrior(string name) : base(name, health: 120, attackPower: 18, defense: 8)
        {
            _rage = 0;
        }

        // ===== ПОЛИМОРФИЗЪМ =====
        // Override на виртуалния метод от базовия клас
        public override int Attack()
        {
            _rage++;
            int bonus = _rage >= 3 ? 10 : 0; // На всеки 3 удара - ярост удар
            if (bonus > 0)
            {
                Console.WriteLine($"  ⚔️  {Name} влиза в ЯРОСТ! Бонус +{bonus} щета!");
                _rage = 0;
            }
            return AttackPower + bonus;
        }

        // Override на абстрактния метод
        public override string GetClassInfo()
        {
            return "⚔️  ВОИН - Висок живот и защита. На всеки 3 удара нанася бонус щета от ЯРОСТ.";
        }
    }

    // ===== НАСЛЕДЯВАНЕ =====
    // Mage наследява Character
    public class Mage : Character
    {
        private int _mana;
        private int _maxMana;

        public Mage(string name) : base(name, health: 80, attackPower: 25, defense: 3)
        {
            _mana = 60;
            _maxMana = 60;
        }

        // ===== ПОЛИМОРФИЗЪМ =====
        public override int Attack()
        {
            if (_mana >= 20)
            {
                _mana -= 20;
                Console.WriteLine($"  🔮 {Name} хвърля ОГНЕНА ТОПКА! (Мана: {_mana}/{_maxMana})");
                return AttackPower + 10; // Магически бонус
            }
            else
            {
                Console.WriteLine($"  🔮 {Name} няма достатъчно мана - обикновен удар!");
                return AttackPower - 5;
            }
        }

        public override string GetClassInfo()
        {
            return "🔮 МАГ - Висока атака, нисък живот. Харчи мана за мощни заклинания.";
        }
    }

    // ===== НАСЛЕДЯВАНЕ =====
    // Rogue наследява Character
    public class Rogue : Character
    {
        private Random _rng = new Random();

        public Rogue(string name) : base(name, health: 95, attackPower: 20, defense: 5)
        {
        }

        // ===== ПОЛИМОРФИЗЪМ =====
        public override int Attack()
        {
            bool critical = _rng.Next(0, 100) < 30; // 30% шанс за критичен удар
            if (critical)
            {
                Console.WriteLine($"  🗡️  {Name} нанася КРИТИЧЕН УДАР!");
                return AttackPower * 2;
            }
            return AttackPower;
        }

        public override string GetClassInfo()
        {
            return "🗡️  РАЗБОЙНИК - Балансиран герой. 30% шанс за критичен удар x2 щета.";
        }
    }

    // ===== НАСЛЕДЯВАНЕ =====
    // Monster наследява Character (базов клас за чудовища)
    public class Monster : Character
    {
        private string _type;

        public Monster(string name, int health, int attack, int defense, string type)
            : base(name, health, attack, defense)
        {
            _type = type;
        }

        public override string GetClassInfo()
        {
            return $"Чудовище от тип: {_type}";
        }
    }
}
