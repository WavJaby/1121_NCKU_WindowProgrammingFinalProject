namespace Project.lib;

public static class MathHelper {
    public static float DegToRad(float degrees) => MathF.PI / 180f * degrees;
    public static float RadToDeg(double radians) => (float) (180f / Math.PI * radians);
}