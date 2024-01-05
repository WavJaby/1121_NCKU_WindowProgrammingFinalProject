using Project.GameHelper;

namespace Project.defaultObjects;

public class Group : BaseGameObject {
    public readonly List<GameObject> GameObjects = new();

    public void AddGameObject(GameObject gameObject) {
        GameObjects.Add(gameObject);
    }

    public void RemoveGameObjectAt(List<int> index) {
        for (int i = index.Count - 1; i >= 0; i--) {
            GameObjects[i].Dispose();
            GameObjects.Remove(GameObjects[i]);
        }
    }

    public override void Render(Scene scene) {
        foreach (var gameObject in GameObjects) {
            gameObject.Render(scene);
        }
    }

    public override void Dispose() {
        foreach (var gameObject in GameObjects) {
            gameObject.Dispose();
        }
    }
}