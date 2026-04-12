using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace DungeonSlime;

public class Game1 : Core
{
    private AnimatedSprite _slime;
    private AnimatedSprite _bat;

    private Vector2 _slimePosition;
    private Vector2 _batPosition;
    private Vector2 _batVelocity;

    private Tilemap _tilemap;

    private Rectangle _roomBounds;

    private const float MOVEMENT_SPEED = 5.0f;
    private const float SPEED_MULTIPLIER = 2.0f;

    private SoundEffect _bounceSoundEffect;
    private SoundEffect _collectSoundEffect;

    private Song _themeSong;

    public Game1() : base("Dungeon Slime", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();

        _batPosition = new Vector2(_slime.Width + 10, 0);

        Rectangle screenBounds = GraphicsDevice.PresentationParameters.Bounds;

        _roomBounds = new Rectangle(
            (int)_tilemap.TileWidth,
            (int)_tilemap.TIleHeight,
            screenBounds.Width - (int)_tilemap.TileWidth * 2,
            screenBounds.Height - (int)_tilemap.TIleHeight * 2
         );

        int centerRow = _tilemap.Rows / 2;
        int centerColumn = _tilemap.Columns / 2;

        _slimePosition = new Vector2(centerColumn * _tilemap.TileWidth, centerRow * _tilemap.TIleHeight);
        _batPosition = new Vector2(_roomBounds.Left, _roomBounds.Top);

        AssignRandomBatVelocity();

        Audio.PlaySong(_themeSong);
    }

    protected override void LoadContent()
    {
        // Инициализация тайлмапа
        _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);

        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(4.0f, 4.0f);

        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(4f, 4f);

        _bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        _themeSong = Content.Load<Song>("audio/theme");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _slime.Update(gameTime);
        _bat.Update(gameTime);

        CheckKeyboardInput();
        CheckGamePadInput();


        // Creating a bounding circle for the slime
        Circle slimeBounds = new Circle(
            (int)(_slimePosition.X + (_slime.Width * 0.5f)),
            (int)(_slimePosition.Y + (_slime.Height * 0.5f)),
            (int)(_slime.Width * 0.5f)
        );

        // Проверка на коллизию с краями экрана, основанная на оценке расстояния
        // при столкновении - движение в обратном направлении
        if (slimeBounds.Left < _roomBounds.Left)
        {
            _slimePosition.X = _roomBounds.Left;
        }
        else if (slimeBounds.Right > _roomBounds.Right)
        {
            _slimePosition.X = _roomBounds.Right - _slime.Width;
        }

        if (slimeBounds.Top < _roomBounds.Top)
        {
            _slimePosition.Y = _roomBounds.Top;
        }
        else if (slimeBounds.Bottom > _roomBounds.Bottom)
        {
            _slimePosition.Y = _roomBounds.Bottom - _slime.Height;
        }

        // Расчет новой позиции мыши 
        Vector2 newBatPosition = _batPosition + _batVelocity;

        // Границы мышки
        Circle batBounds = new Circle(
            (int)(newBatPosition.X + (_bat.Width * 0.5f)),
            (int)(newBatPosition.Y + (_bat.Height * 0.5f)),
            (int)(_bat.Width * 0.5f)
        );

        Vector2 normal = Vector2.Zero;

        // Основанная на оценке расстояния проверка коллизии мыши с краями экрана
        if (batBounds.Left < _roomBounds.Left)
        {
            normal.X = Vector2.UnitX.X;
            newBatPosition.X = _roomBounds.Left;
        }
        else if (batBounds.Right > _roomBounds.Right)
        {
            normal.X = -Vector2.UnitX.X;
            newBatPosition.X = _roomBounds.Right - _bat.Width;
        }

        if (batBounds.Top < _roomBounds.Top)
        {
            normal.Y = Vector2.UnitY.Y;
            newBatPosition.Y = _roomBounds.Top;
        }
        else if (batBounds.Bottom > _roomBounds.Bottom)
        {
            normal.Y = -Vector2.UnitY.Y;
            newBatPosition.Y = _roomBounds.Bottom - _bat.Height;
        }

        // If the normal is anything but Vector2.Zero, this means the bat had
        // moved outside the screen edge so we should reflect it about the
        // normal.
        if (normal != Vector2.Zero)
        {
            normal.Normalize();
            _batVelocity = Vector2.Reflect(_batVelocity, normal);

            Audio.PlaySoundEffect(_bounceSoundEffect);
        }

        _batPosition = newBatPosition;

        if (slimeBounds.Intersects(batBounds))
        {
            // Choose a random row and column based on the total number of each
            int column = Random.Shared.Next(1, _tilemap.Columns - 1);
            int row = Random.Shared.Next(1, _tilemap.Rows - 1);

            _batPosition = new Vector2(column * _bat.Width, row * _bat.Height);

            AssignRandomBatVelocity();

            Audio.PlaySoundEffect(_collectSoundEffect);
        }

        base.Update(gameTime);
    }

    private void AssignRandomBatVelocity()
    {
        // Рандомный угол
        float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        // Преобразуем угол в вектор
        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new Vector2(x, y);

        _batVelocity = direction * MOVEMENT_SPEED;
    }

    private void CheckKeyboardInput()
    {
        // Get the state of keyboard input
        KeyboardState keyboardState = Keyboard.GetState();

        float speed = MOVEMENT_SPEED;

        if (Input.Keyboard.IsKeyDown(Keys.Space))
        {
            speed *= SPEED_MULTIPLIER;
        }

        if (Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up))
        {
            _slimePosition.Y -= speed;
        }
        
        if (Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left))
        {
            _slimePosition.X -= speed;
        }

        if (Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down))
        {
            _slimePosition.Y += speed;
        }

        if (Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right))
        {
            _slimePosition.X += speed;
        }

        // Toggle Mute state on 'M' pressed
        if (Input.Keyboard.WasKeyJustPressed(Keys.M))
        {
            Audio.ToggleMute();
        }

        if (Input.Keyboard.WasKeyJustPressed(Keys.OemPlus))
        {
            Audio.SongVolume += 0.1f;
            Audio.SoundEffectVolume += 0.1f;
        }

        if (Input.Keyboard.WasKeyJustPressed(Keys.OemMinus))
        {
            Audio.SongVolume -= 0.1f;
            Audio.SoundEffectVolume -= 0.1f;
        }
    }

    private void CheckGamePadInput()
    {
        GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

        GamePadInfo gamePadOne = Input.GamePads[(int)PlayerIndex.One];

        float speed = MOVEMENT_SPEED;

        if (gamePadOne.IsButtonDown(Buttons.A))
        {
            speed *= SPEED_MULTIPLIER;
            gamePadOne.SetVibration(1.0f, TimeSpan.FromSeconds(1));
        }
        else
        {
            gamePadOne.StopVibration();
        }

        if (gamePadOne.LeftThumbStick != Vector2.Zero)
        {
            _slimePosition.X += gamePadOne.LeftThumbStick.X * speed;
            _slimePosition.Y += gamePadOne.LeftThumbStick.Y * speed;
        }
        else
        {
            if (gamePadOne.IsButtonDown(Buttons.DPadDown))
            {
                _slimePosition.Y -= speed;
            }

            if (gamePadOne.IsButtonDown(Buttons.DPadLeft))
            {
                _slimePosition.X -= speed;
            }

            if (gamePadOne.IsButtonDown(Buttons.DPadUp))
            {
                _slimePosition.Y += speed;
            }

            if (gamePadOne.IsButtonDown(Buttons.DPadRight))
            {
                _slimePosition.X += speed;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _tilemap.Draw(SpriteBatch);
        _slime.Draw(SpriteBatch, _slimePosition);
        _bat.Draw(SpriteBatch, _batPosition);

        SpriteBatch.End();

        base.Draw(gameTime);
    }
}