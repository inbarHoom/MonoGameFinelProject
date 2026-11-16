namespace MonoGameFinelProject;
using Microsoft.Xna.Framework;
using System;

public class AnimationClip
{
    public int StartColumn;
    public int StartRow;
    public int FrameCount;
    public int FramesPerSecond;
    public bool Loop;

    public AnimationClip(int startColumn, int startRow, int frameCount, int framesPerSecond, bool loop)
    {
        StartColumn = startColumn;
        StartRow = startRow;
        FrameCount = frameCount;
        FramesPerSecond = framesPerSecond;
        Loop = loop;
    }
}

public class Animation : Sprite
{
    public int samples = 60;
    private bool holdingLastFrame;
    private double lastFrameHoldElapsed;
    private double lastFrameHoldSeconds;
    private int _fixedRow = -1;
    private int _rowFrameCount;

    public bool isFinished { get; private set; }
    private double totalTime = 0;
    private bool looping = true;
    private bool isAnimating = false;
    private int x = 0;
    private int y = 0;
    
    private int currentClipFrame = 0;
    private AnimationClip currentClip;

    public int CurrentFrame => currentClip == null ? x : currentClipFrame;
    public Animation(string SpriteName) : base(SpriteName)
    {
    }
    public void PlayClip(AnimationClip clip)
    {
        if (clip == null)
            throw new ArgumentNullException(nameof(clip));

        int firstCell = clip.StartRow * _spritesheet.columns + clip.StartColumn;
        int totalCells = _spritesheet.rows * _spritesheet.columns;

        if (clip.StartColumn < 0 || clip.StartColumn >= _spritesheet.columns ||
            clip.StartRow < 0 || clip.StartRow >= _spritesheet.rows ||
            clip.FrameCount <= 0 ||
            firstCell + clip.FrameCount > totalCells)
        {
            throw new ArgumentOutOfRangeException(nameof(clip),
                "The clip frames must fit within the spritesheet.");
        }
        currentClip = clip;
        currentClipFrame = 0;
        looping = clip.Loop;
        samples = clip.FramesPerSecond;
        totalTime = 0;
        lastFrameHoldSeconds = 0;
        lastFrameHoldElapsed = 0;
        holdingLastFrame = false;
        isFinished = false;
        isAnimating = true;

        SetClipFrame();
    }

    private void SetClipFrame()
    {
        int firstCell = currentClip.StartRow * _spritesheet.columns + currentClip.StartColumn;

        int cell = firstCell + currentClipFrame;
        int column = cell % _spritesheet.columns;
        int row = cell / _spritesheet.columns;

        _sourceRectangle = _spritesheet[column, row];
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
        currentClip = null;
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
        currentClip = null;
        _spritesheet = SpriteManager.GetSprite(newAnimation);
        _texture = _spritesheet.texture;
        _fixedRow = -1;
        Reset();
    }

    protected void StartRowAnimation(int row, int frameCount, bool loops, int framesPerSecond, double lastFrameHoldSeconds = 0)
    {
        if (row < 0 || row >= _spritesheet.rows)
            throw new ArgumentOutOfRangeException(nameof(row));

        if (frameCount <= 0 || frameCount > _spritesheet.columns)
            throw new ArgumentOutOfRangeException(nameof(frameCount));

        _fixedRow = row;
        _rowFrameCount = frameCount;

        x = 0;
        y = row;
        totalTime = 0;
        isAnimating = false;
        holdingLastFrame = false;
        lastFrameHoldElapsed = 0;
        isFinished = false;

        looping = loops;
        samples = framesPerSecond;
        this.lastFrameHoldSeconds = lastFrameHoldSeconds;

        _sourceRectangle = _spritesheet[x, y];
        isAnimating = true;
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
        if (currentClip != null)
        {
            totalTime = 0;
            currentClipFrame++;

            if (currentClipFrame >= currentClip.FrameCount)
            {
                if (currentClip.Loop)
                {
                    currentClipFrame = 0;
                }
                else
                {
                    currentClipFrame = currentClip.FrameCount - 1;
                    isAnimating = false;
                    isFinished = true;
                    SetClipFrame();
                    return;
                }
            }

            SetClipFrame();
            return;
        }
        totalTime = 0;
        if (_fixedRow >= 0)
        {
            x++;

            if (x >= _rowFrameCount)
            {
                if (looping)
                {
                    x = 0;
                }
                else
                {
                    x = _rowFrameCount - 1;
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

                    _sourceRectangle = _spritesheet[x, _fixedRow];
                    return;
                }
            }

            y = _fixedRow;
            _sourceRectangle = _spritesheet[x, y];
            return;
        }
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
            MoveFrame();
        }
        
        base.Update(gameTime);
    }
    
}