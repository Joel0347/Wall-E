using System.Data;
using System.Text.RegularExpressions;
namespace Hulk;

// Clase que evalúa los strings
public class String : Types
{
    // Método para ignorar strings, convirtiendolos en espacios en blanco
    public static string StringsToSpaces(string s) {

        int index1 = s.IndexOf("\"");
        int index2 = s.IndexOf("\"", index1 + 1);
        int count1 = 0;
        int count2 = 0;

        // Se itera mientras tenga comillas
        while (index1 != -1) {

            // Se revisa si la comilla inicial lleva slash 
            for (int i = index1; i > 0; i--) {
                if (s[i - 1] != '\\') break;
                count1 ++;
            }

            // Se revisa si la comilla final lleva slash 
            for (int i = index2; i > 0; i--) {
                if (s[i - 1] != '\\') break;
                count2 ++;
            }

            /* Si ambas comillas tienen un número par de slashes (o ninguno) 
            se toman como delimitantes de strings y se sustituye por espacios 
            en blanco, respetando el length */
            if (count1 % 2 == 0 && count2 % 2 == 0) {
                
                s = s[..(index1 + 1)] + s[index2..]; 
                int dif = index2 - index1 - 1;
                string spaces = new(' ', dif);
                s = s.Insert(index1 + 1, spaces);

                index1 = s.IndexOf("\"", index2 + 1);  
                index2 = s.IndexOf("\"", index1 + 1);
            }

            /* En caso que sea impar entonces la comilla es interna del string
            y no debe contarse. Por tanto se busca la siguiente */
            else if (count1 % 2 != 0) { 
                index1 = index2;
                index2 = s.IndexOf("\"", index1 + 1);
            }

            else index2 = s.IndexOf("\"", index2 + 1);
            
            count1 = 0;
            count2 = 0;
        }

        // Se retorna un string del mismo length que la entrada pero en blanco
        return s;
    }
    
    // Método que verifica si la entrada es un string
    public static bool IsString(string s) {
        s = StringsToSpaces(s);
        s = s.Replace(" ","");
      
        return s == "\"\"" || s.Contains("@");
    }

    // Método que evalúa el string
    public static string Eval(string s)
    {
        string n = StringsToSpaces(s);
        
        // Si contiene el operador '@' se evalúa
        if (n.Contains("@")) {
            int index = n.LastIndexOf("@");   
            return Concatenate.Eval(Main.Parse(s[..index]), Main.Parse(s[(index + 1)..]));
        }
        
        // Se retorna el string evaluado
        return s;
    }

    // Método que evalúa los slash en el string
    public static string SlashEval(string s) {
        // Secuencias de escape permitidas
        char[] scapes = {'n', 'r', 't', 'a', 'f', 'b', 'v', '"', '\'', '\\'};
        char[] scapeSequency = {'\n', '\r', '\t', '\a', '\f', '\b', '\v'};

        int index = s.IndexOf("\\"); 

        // Se itera mientras tenga slash
        while (index != -1) {
            int count = 0;

            // Se cuentan cuántos seguidos hay
            for (int i = index; i < s.Length; i++) {
                if (s[i] != '\\') break;
                count ++;
            }

            // Se remueve la mitad de los slash
            s = s.Remove(index, count / 2);

            // Si no es par, se inserta el caracter de escape deseado
            if (count % 2 != 0) {
                int scapeIndex  = Array.IndexOf(scapes, s[index + count - count / 2]);
                s = s.Remove(index, 1);

                if (!(scapes[scapeIndex] == '"' || scapes[scapeIndex] == '\'' || 
                    scapes[scapeIndex] == '\\')) 
                {
                    s = s.Remove(index + count - count / 2 - 1, 1);
                    s = s.Insert(index + count - count / 2 - 1, scapeSequency[scapeIndex].ToString());
                }
            }
            
            index = s.IndexOf("\\", index + count / 2);
        }
        
        return s;
    }
}