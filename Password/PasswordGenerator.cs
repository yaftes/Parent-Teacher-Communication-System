public static class Password {
     public static string GeneratePassword(){
        Random randomNumber = new Random();
        char[] passwordChars = new char[6];
        for (int i = 0; i < 2; i++)
        {
            passwordChars[i] = (char)randomNumber.Next('A', 'Z' + 1);
        }
        for (int i = 2; i < 4; i++)
        {
            passwordChars[i] = (char)randomNumber.Next('a', 'z' + 1);
        }
        for (int i = 4; i < 6; i++)
        {
            passwordChars[i] = (char)randomNumber.Next('0', '9' + 1);
        }
        passwordChars = passwordChars.OrderBy(x => randomNumber.Next()).ToArray();

        return new string(passwordChars);
    }

}