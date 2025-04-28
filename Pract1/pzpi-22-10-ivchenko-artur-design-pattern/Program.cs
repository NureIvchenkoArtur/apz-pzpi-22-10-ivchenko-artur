public class Document
{
    private string state;
    private User currentUser;

    public Document(User user)
    {
        currentUser = user;
        state = "draft"; // Початковий стан
    }

    public void Publish()
    {
        switch (state)
        {
            case "draft":
                state = "moderation";
                break;
            case "moderation":
                if (currentUser.Role == "admin")
                {
                    state = "published";
                }
                break;
            case "published":
                // Нічого не робимо
                break;
        }
    }

    public string GetState()
    {
        return state;
    }
}

public class User
{
    public string Role { get; set; }

    public User(string role)
    {
        Role = role;
    }
}

// Загальний інтерфейс станів
public abstract class State
{
    protected AudioPlayer player;

    public State(AudioPlayer player)
    {
        this.player = player;
    }

    public abstract void ClickLock();
    public abstract void ClickPlay();
    public abstract void ClickNext();
    public abstract void ClickPrevious();
}

// Конкретний стан: LockedState
public class LockedState : State
{
    public LockedState(AudioPlayer player) : base(player) { }

    public override void ClickLock()
    {
        if (player.IsPlaying)
            player.ChangeState(new PlayingState(player));
        else
            player.ChangeState(new ReadyState(player));
    }

    public override void ClickPlay()
    {
        // Нічого не робити
    }

    public override void ClickNext()
    {
        // Нічого не робити
    }

    public override void ClickPrevious()
    {
        // Нічого не робити
    }
}

// Конкретний стан: ReadyState
public class ReadyState : State
{
    public ReadyState(AudioPlayer player) : base(player) { }

    public override void ClickLock()
    {
        player.ChangeState(new LockedState(player));
    }

    public override void ClickPlay()
    {
        player.StartPlayback();
        player.ChangeState(new PlayingState(player));
    }

    public override void ClickNext()
    {
        player.NextSong();
    }

    public override void ClickPrevious()
    {
        player.PreviousSong();
    }
}

// Конкретний стан: PlayingState
public class PlayingState : State
{
    public PlayingState(AudioPlayer player) : base(player) { }

    public override void ClickLock()
    {
        player.ChangeState(new LockedState(player));
    }

    public override void ClickPlay()
    {
        player.StopPlayback();
        player.ChangeState(new ReadyState(player));
    }

    public override void ClickNext()
    {
        if (player.IsDoubleClick)
            player.NextSong();
        else
            player.FastForward(5);
    }

    public override void ClickPrevious()
    {
        if (player.IsDoubleClick)
            player.PreviousSong();
        else
            player.Rewind(5);
    }
}

// Контекст — AudioPlayer
public class AudioPlayer
{
    private State state;
    public bool IsPlaying { get; private set; }
    public bool IsDoubleClick { get; set; }

    public AudioPlayer()
    {
        state = new ReadyState(this);
        IsPlaying = false;
    }

    public void ChangeState(State newState)
    {
        state = newState;
    }

    // Методи взаємодії UI
    public void ClickLock()
    {
        state.ClickLock();
    }

    public void ClickPlay()
    {
        state.ClickPlay();
    }

    public void ClickNext()
    {
        state.ClickNext();
    }

    public void ClickPrevious()
    {
        state.ClickPrevious();
    }

    // Сервісні методи
    public void StartPlayback()
    {
        Console.WriteLine("▶️ Відтворення розпочато");
        IsPlaying = true;
    }

    public void StopPlayback()
    {
        Console.WriteLine("⏹️ Відтворення зупинено");
        IsPlaying = false;
    }

    public void NextSong()
    {
        Console.WriteLine("⏭️ Наступна пісня");
    }

    public void PreviousSong()
    {
        Console.WriteLine("⏮️ Попередня пісня");
    }

    public void FastForward(int seconds)
    {
        Console.WriteLine($"⏩ Промотка вперед на {seconds} секунд");
    }

    public void Rewind(int seconds)
    {
        Console.WriteLine($"⏪ Промотка назад на {seconds} секунд");
    }
}

// Приклад використання
public class Program
{
    public static void Main(string[] args)
    {
        User adminUser = new User("admin");
        Document doc = new Document(adminUser);

        Console.WriteLine($"Початковий стан: {doc.GetState()}");
        doc.Publish();
        Console.WriteLine($"Стан після публікації: {doc.GetState()}");
        doc.Publish();
        Console.WriteLine($"Стан після другої публікації: {doc.GetState()}");
        
        AudioPlayer player = new AudioPlayer();

        player.ClickPlay();      // Стартуємо відтворення
        player.ClickNext();      // Перемикаємо пісню
        player.ClickLock();      // Блокуємо
        player.ClickPlay();      // Нічого не відбувається
        player.ClickLock();      // Розблоковуємо
        player.ClickPlay();      // Ставимо на паузу
    }
}