using lab07;

var fileNameEncrypt = "encrypt_eee3.txt";
var fileNameDecrypt = "decrypt_eee3.txt";
var firstKey = CypherHelper.GetBytes("myKeyDES");
var secondKey = CypherHelper.GetBytes("otherKey");
var thirdKey = CypherHelper.GetBytes("blueBoxM");
var plainText = CypherHelper.GetOpenText();
int totalBits, changedBits;


var encryptedText = Cypher.EncryptEEE3(plainText, firstKey, secondKey, thirdKey, out changedBits);
CypherHelper.WriteToFile(encryptedText, fileNameEncrypt);

var decryptedText = Cypher.DecryptEEE3(encryptedText, firstKey, secondKey, thirdKey);
CypherHelper.WriteToFile(decryptedText, fileNameDecrypt);

Console.WriteLine();
Console.WriteLine($"Total bits count:\t{totalBits = CypherHelper.GetTotalBits(plainText)} bits");
Console.WriteLine($"Avalanche Effect:\t{changedBits} bits (changed)");
Console.WriteLine($"Percentage ratio:\t{CypherHelper.GetPercentageRatio(totalBits, changedBits)}%");