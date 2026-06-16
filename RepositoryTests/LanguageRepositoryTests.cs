using Repository;

namespace RepositoryTests
{
    public class LanguageRepositoryTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("&%")]
        public void Test_Language_Greeting_Not_Found(string code)
        {
            var languageRepository = new LanguageRepository();
            var result = languageRepository.GetGreetingMessage(code);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("es","Hola")]
        [InlineData("ES","Hola")]
        [InlineData("Es", "Hola")]
        public void Test_Language_Greeting(string code,string output)
        {
            var languageRepository = new LanguageRepository();
            var result = languageRepository.GetGreetingMessage(code);

            Assert.Equal(output, result);
        }
    }
}
