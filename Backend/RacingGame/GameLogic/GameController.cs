using System.Numerics;

namespace GameLogic
{
    public class GameController
    {
        public HashSet<GameAgent> Agents = [];
        public HashSet<Vector2> Track = [];
        public HashSet<Vector2> TrackRaw = [];

        public int TrackLength { get; set; }

        public int TrackWidth { get; set; } = 200;

        public int TrackMaxDeltaX { get; set; } = 300;
        public int TrackMaxDeltaY { get; set; } = 20;

        public void AddAgent(GameAgent agent)
        {
            Agents.Add(agent);
        }

        public void RemoveAgent(GameAgent agent)
        {
            Agents.Remove(agent);
        }

        public void ClearAgents()
        {
            Agents.Clear();
        }

        public int GetAgentCount()
        {
            return Agents.Count;
        }

        public void Update(float deltaTime)
        {
            foreach (GameAgent agent in Agents)
            {
                agent.Update(deltaTime);
            }
        }

        public void GenerateTrack(int length, int seed)
        {
            var rnd = new Random(seed);
            TrackLength = length;
            Track.Clear();
            Vector2 pointBeforeLastPoint = new Vector2(0, -10);
            Vector2 lastPoint = new Vector2(0, 0);
            Track.Add(lastPoint);
            
            for (int i = 0; i < length; i++)
            {
                int deltaX = rnd.Next(-TrackMaxDeltaX, TrackMaxDeltaX + 1);
                int deltaY = (int)Math.Pow(
                    rnd.Next((int)Math.Sqrt(Math.Abs(deltaX)), Math.Max(TrackMaxDeltaY + 1, deltaX) + 1)
                    , 2);
                deltaY = 400;

                Vector2 newPoint = lastPoint + new Vector2(deltaX, deltaY);

                int segments = (int)(newPoint.Length() / 10);

                Vector2 fromLastPointToHalf = Vector2.Normalize(lastPoint - pointBeforeLastPoint) * new Vector2(deltaX, deltaY).Length() / 2;
                Vector2 half = fromLastPointToHalf + lastPoint;

                for (int j = 0; j < segments; j++)
                {
                    Vector2 a, b;
                    a = Vector2.Lerp(lastPoint, half, (float)j / segments);
                    b = Vector2.Lerp(half, newPoint, (float)j / segments);
                    Track.Add(Vector2.Lerp(a, b, (float)j / segments));
                }
                TrackRaw.Add(newPoint);
                pointBeforeLastPoint = Track.Last();
                lastPoint = newPoint;
            }
        }
    }
}
