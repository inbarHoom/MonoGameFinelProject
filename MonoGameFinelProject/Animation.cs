namespace MonoGameFinelProject;
using Microsoft.Xna.Framework;
public class Animation : Sprite
{
    public int samples = 60;
    private bool holdingLastFrame;
    private double lastFrameHoldElapsed;
    private double lastFrameHoldSeconds;

    public bool isFinished { get; private set; }
    private double totalTime = 0;
    private bool looping = true;
    private bool isAnimating = false;
    private int x = 0;
    private int y = 0;
    public int CurrentFrame => x;
    public Animation(string SpriteName) : base(SpriteName)
    {
    }

    public void PauseAnimation()
    {
        isAnimating = false;
    }
    
    public void ResumeAnimation()
    {
        if(!isFinished)
        isAnimating = true;
    }

    public void StopAnimation()
    {
        Reset();
    }
    
    public void PlayAnimation(bool isLooping = true, int samples = 60, double lastFrameHoldSeconds = 0)
    {
        this.looping = isLooping;
        this.samples = samples;
        this.lastFrameHoldSeconds = lastFrameHoldSeconds;
        Reset();
        isAnimating = true;
    }

    protected virtual void StartAnimation(string spriteName, bool loops, int framesPerSecond, double lastFrameHoldSeconds)
    {
        ChangeAnimation(spriteName);
        PlayAnimation(loops, framesPerSecond, lastFrameHoldSeconds);
    }

    public void ChangeAnimation(string newAnimation)
    {
        _spritesheet = SpriteManager.GetSprite(newAnimation);
        _texture = _spritesheet.texture;
        Reset();
    }

    void Reset()
    {
        x = y = 0;
        totalTime = 0;
        isAnimating = false;
        holdingLastFrame = false;
        lastFrameHoldElapsed = 0;
        isFinished = false;
        _sourceRectangle = _spritesheet[x, y];

    }
    bool CanMoveToNextFrame(GameTime gameTime)
    {
        if (!isAnimating || samples <=0) return false;
        totalTime += gameTime.ElapsedGameTime.TotalSeconds;
        
        double secondsPerFrame = 1.0/ samples;
        if(totalTime < secondsPerFrame) return false;
        totalTime -= secondsPerFrame;
        return true;
    }

    void MoveFrame()
    {
        totalTime = 0;
        x++;
        if (x >= _spritesheet.columns)
        {
            x = 0;
            y++;

            if (y >= _spritesheet.rows)
            {
                if (looping)
                {
                    y = 0;
                    x = 0;
                }
                else
                {
                    x = _spritesheet.columns - 1;
                    y = _spritesheet.rows - 1;
                    isAnimating = false;
                    if (lastFrameHoldSeconds > 0)
                    {
                        holdingLastFrame = true;
                        lastFrameHoldElapsed = 0;
                    }
                    else
                    {
                        isFinished = true;
                    }
                    return;
                }
            }
        }
        _sourceRectangle = _spritesheet[x, y];
    }


    public override void Update(GameTime gameTime)
    {
        if (holdingLastFrame)
        {
            lastFrameHoldElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            if (lastFrameHoldElapsed >= lastFrameHoldSeconds)
            {
                holdingLastFrame = false;
                isAnimating = false;
                isFinished = true;
            }
        }
        else if (CanMoveToNextFrame(gameTime))
        {
            // Switch to next frame
            MoveFrame();
        }
        
        // next lecture - I will explain why it needs to be the last thing here
        base.Update(gameTime);
    }
    
}