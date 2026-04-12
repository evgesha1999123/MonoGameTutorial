using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;

namespace test.Scenes;

public class TitleScene: Scene
{
    private const string DUNGEON_TEXT = "Dungeon";
    private const string SLIME_TEXT = "Slime";
    private const string PRESS_ENTER_TEXT = "Press \"Enter\" To Start";
    private const string PRESS_ESC_TO_EXIT = "Press \"ESC\" To Exit";

    private SpriteFont _font;
    private SpriteFont _font5x;

    private Vector2 _dungeonTextPos;
    private Vector2 _dungeonTextOrigin;
    private Vector2 _slimeTextPos;
    private Vector2 _slimeTextOrigin;
    private Vector2 _pressEnterPos;
    private Vector2 _pressEnterOrigin;
    private Vector2 _pressEscPos;
    private Vector2 _pressEscOrigin;

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // While on the title screen, we can enable exit on escape so the player
        // can close the game by pressing the escape key.
        Core.ExitOnEscape = true;

        // Set the position and origin for the Dungeon text.
        Vector2 size = _font5x.MeasureString(DUNGEON_TEXT);
        _dungeonTextPos = new Vector2(640, 100);
        _dungeonTextOrigin = size * 0.5f;

        // Set the position and origin for the Slime text
        size = _font5x.MeasureString(SLIME_TEXT);
        _slimeTextPos = new Vector2(757, 207);
        _slimeTextOrigin = size * 0.5f;

        // Set the position and origin for the press enter text
        size = _font.MeasureString(PRESS_ENTER_TEXT);
        _pressEnterPos = new Vector2(640, 520);
        _pressEnterOrigin = size * 0.5f;

        // Set the position and origin for the press esc text
        size = _font.MeasureString(PRESS_ESC_TO_EXIT);
        _pressEscPos = new Vector2(_pressEnterPos.X, _pressEnterPos.Y + 50.0f);
        _pressEscOrigin = size * 0.5f;
    }

    public override void LoadContent()
    {
        _font = Core.Content.Load<SpriteFont>("fonts/04B_30");
        _font5x = Core.Content.Load<SpriteFont>("fonts/04B_30_5x");
    }

    public override void Update(GameTime gameTime)
    {
        // If the user presses enter, switch to the game scene
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            Core.ChangeScene(new GameScene());
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(32, 40, 78, 255));

        // Begin the sprite batch to prepare for rendering
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // The color to use for the drop shadow text
        Color dropShadowColor = Color.Black * 0.5f;

        // Draw the Dungeon text slightly offset from it is original position and
        // with a transparent color to give it a drop shadow
        Core.SpriteBatch.DrawString(_font5x, DUNGEON_TEXT, _dungeonTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _dungeonTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw the Dungeon text on the top of that at its original position
        Core.SpriteBatch.DrawString(_font5x, DUNGEON_TEXT, _dungeonTextPos, Color.White, 0.0f, _dungeonTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw the Slime text slightly offset from it is original position and with a transparent color to give it a drop shadow
        Core.SpriteBatch.DrawString(_font5x, SLIME_TEXT, _slimeTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _slimeTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw the Slime text on top of that at its original position
        Core.SpriteBatch.DrawString(_font5x, SLIME_TEXT, _slimeTextPos, Color.White, 0.0f, _slimeTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw the Press Enter Text shadow
        Core.SpriteBatch.DrawString(_font, PRESS_ENTER_TEXT, _pressEnterPos + new Vector2(5, 5), dropShadowColor, 0.0f, _pressEnterOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw The Press Enter Text itself
        Core.SpriteBatch.DrawString(_font, PRESS_ENTER_TEXT, _pressEnterPos, Color.White, 0.0f, _pressEnterOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw The press Esc Text shadow
        Core.SpriteBatch.DrawString(_font, PRESS_ESC_TO_EXIT, _pressEscPos + new Vector2(5, 5), dropShadowColor, 0.0f, _pressEscOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw the press Esc Text itself
        Core.SpriteBatch.DrawString(_font, PRESS_ESC_TO_EXIT, _pressEscPos, Color.White, 0.0f, _pressEscOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();
    }
}

