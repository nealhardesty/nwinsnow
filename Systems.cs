using System.Collections.Concurrent;

namespace NWinSnow;

internal sealed class SnowParticle
{
    public float X;
    public float Y;
    public float SizeScale;
    public float Speed;
    public int TextureIndex;
}

internal sealed class SnowSystem
{
    private readonly Config.AppConfig _config;
    private readonly Assets _assets;
    private readonly List<SnowParticle> _particles = new();
    private readonly Random _random = new();
    private float _spawnAccumulator;
    private int _maxActive;
    private float _elapsedSinceStart;
    private const float RampDurationSeconds = 10.0f;

    public SnowSystem(Config.AppConfig config, Assets assets)
    {
        _config = config;
        _assets = assets;
        _maxActive = _config.Snow.MaxSnowflakes;
    }

    public void ApplyPowerSave(bool enabled)
    {
        _maxActive = enabled ? Math.Max(25, _config.Snow.MaxSnowflakes / 2) : _config.Snow.MaxSnowflakes;
        if (_particles.Count > _maxActive)
        {
            _particles.RemoveRange(_maxActive, _particles.Count - _maxActive);
        }
    }

    public void Update(float dt, Size clientSize, float windHorizontal)
    {
        _elapsedSinceStart += dt;

        // Time-based ramp: reach full count by ~10s
        int desiredCount = (int)MathF.Round(_maxActive * Math.Clamp(_elapsedSinceStart / RampDurationSeconds, 0f, 1f));
        while (_particles.Count < desiredCount)
        {
            _particles.Add(CreateParticle(clientSize.Width));
        }

        _spawnAccumulator += _config.Snow.SpawnRate * dt;
        while (_spawnAccumulator >= 1f && _particles.Count < _maxActive)
        {
            _spawnAccumulator -= 1f;
            _particles.Add(CreateParticle(clientSize.Width));
        }

        for (int i = 0; i < _particles.Count; i++)
        {
            var p = _particles[i];
            p.Y += (p.Speed + _config.Snow.Speed) * dt * 20f;
            p.X += windHorizontal * dt * (0.5f + p.SizeScale);

            if (p.X < -32) p.X += clientSize.Width + 64;
            if (p.X > clientSize.Width + 32) p.X -= clientSize.Width + 64;

            if (p.Y > clientSize.Height + 16)
            {
                p.Y = -16;
                p.X = (float)_random.NextDouble() * clientSize.Width;
            }
        }
    }

    private SnowParticle CreateParticle(int width)
    {
        return new SnowParticle
        {
            X = (float)_random.NextDouble() * width,
            Y = -16f * (float)_random.NextDouble(),
            SizeScale = 0.5f + (float)_random.NextDouble() * 0.5f,
            Speed = 10f * (float)Math.Pow(_random.NextDouble(), 2),
            TextureIndex = _random.Next(0, _assets.SnowflakeTextures.Length)
        };
    }

    

    public void Draw(Graphics g)
    {
        for (int i = 0; i < _particles.Count; i++)
        {
            var p = _particles[i];
            var img = _assets.SnowflakeTextures[p.TextureIndex];
            var size = new SizeF(img.Width * p.SizeScale, img.Height * p.SizeScale);
            g.DrawImage(img, p.X, p.Y, size.Width, size.Height);
        }
    }
}

internal enum WindState
{
    Calm,
    PhaseIn,
    Active,
    PhaseOut
}

internal sealed class WindSystem
{
    private readonly Config.AppConfig _config;
    private readonly Random _random = new();
    private WindState _state = WindState.Calm;
    private float _stateTime;
    private float _direction = 1f;
    private float _intensity;

    public float CurrentHorizontalForce => _direction * _intensity * _config.Wind.Intensity * 20f;

    public WindSystem(Config.AppConfig config)
    {
        _config = config;
    }

    public void Update(float dt)
    {
        _stateTime += dt;
        switch (_state)
        {
            case WindState.Calm:
                if (_random.Next(0, 100) < _config.Wind.ChancePercent)
                {
                    _state = WindState.PhaseIn;
                    _stateTime = 0f;
                    _direction = _random.Next(0, 2) == 0 ? -1f : 1f;
                }
                _intensity = 0f;
                break;
            case WindState.PhaseIn:
                _intensity = Math.Min(1f, _stateTime / 1.0f);
                if (_intensity >= 1f)
                {
                    _state = WindState.Active;
                    _stateTime = 0f;
                }
                break;
            case WindState.Active:
                if (_stateTime >= _config.Wind.DurationSeconds)
                {
                    _state = WindState.PhaseOut;
                    _stateTime = 0f;
                }
                _intensity = 1f;
                break;
            case WindState.PhaseOut:
                _intensity = Math.Max(0f, 1f - _stateTime / 1.5f);
                if (_intensity <= 0f)
                {
                    _state = WindState.Calm;
                    _stateTime = 0f;
                }
                break;
        }
    }
}

internal sealed class TreesSystem
{
    private readonly Config.AppConfig _config;
    private readonly Assets _assets;
    private readonly List<(PointF Position, SizeF Size)> _trees = new();
    private readonly Random _random = new();
    private Size _lastForSize = Size.Empty;

    public TreesSystem(Config.AppConfig config, Assets assets)
    {
        _config = config;
        _assets = assets;
    }

    public void EnsurePlaced(Size clientSize)
    {
        if (_trees.Count > 0 && _lastForSize == clientSize) return;
        _trees.Clear();
        _lastForSize = clientSize;
        int count = Math.Max(0, Math.Min(36, _config.Trees.Count));
        for (int i = 0; i < count; i++)
        {
            var scale = _config.Trees.ScaleMin + (float)_random.NextDouble() * (_config.Trees.ScaleMax - _config.Trees.ScaleMin);
            var w = _assets.TreeTexture.Width * scale;
            var h = _assets.TreeTexture.Height * scale;
            var x = (float)_random.NextDouble() * (clientSize.Width - w);
            var y = clientSize.Height - h;
            _trees.Add((new PointF(x, y), new SizeF(w, h)));
        }
    }

    public void Draw(Graphics g)
    {
        if (_trees.Count == 0) return;
        foreach (var (pos, size) in _trees)
        {
            g.DrawImage(_assets.TreeTexture, pos.X, pos.Y, size.Width, size.Height);
        }
    }
}


