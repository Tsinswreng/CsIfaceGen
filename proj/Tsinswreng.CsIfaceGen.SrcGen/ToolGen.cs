namespace Tsinswreng.CsIfaceGen.SrcGen;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using System.Threading.Tasks;



public class ToolGen{
	public static str CombinePath(str Path1, str Path2){
		if(Path1.EndsWith("\\") || Path1.EndsWith("/")){
			return Path1 + Path2;
		}else{
			return Path1 + "/" + Path2;
		}
	}

	public static bool IsSubType(
		INamedTypeSymbol candidate,
		INamedTypeSymbol parent,
		Compilation compilation
	){
		if (SymbolEqualityComparer.Default.Equals(candidate, parent)){
			return false;
		}

		// 继承或实现接口
		if (candidate.AllInterfaces.Contains(parent, SymbolEqualityComparer.Default)){
			return true;
		}
		// 父类链
		for (var b = candidate.BaseType; b != null; b = b.BaseType) {
			if (SymbolEqualityComparer.Default.Equals(b, parent)){
				return true;
			}
		}

		return false;
	}

	public static string GetSha256HashBase64Url(string input){
		using var sha256 = SHA256.Create();
		byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

		// 普通 Base64 → URL 安全 Base64
		string base64 = Convert.ToBase64String(hash)
							.Replace('+', '-')
							.Replace('/', '_')
							.TrimEnd('=');
		return base64;
	}

	public static string GetSha256HashHex(string input){
		using SHA256 sha256 = SHA256.Create();
		// 将输入字符串转换为字节数组并计算哈希
		byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

		// 将字节数组转换为十六进制字符串
		StringBuilder builder = new StringBuilder();
		for (int i = 0; i < bytes.Length; i++) {
			builder.Append(bytes[i].ToString("x2")); // 小写十六进制
		}
		return builder.ToString();
	}

	public static str ToIdentifierSafeFullType(str FullType){
		var R = FullType.Replace("global::", "");
		R = R.Replace("_", "__");
		R = R.Replace("<", "_");
		R = R.Replace(">", "_");
		R = R.Replace(".", "_");
		return R;
	}

	#pragma warning disable RS1035
	public static void EnsureFile(string filePath) {
		if (string.IsNullOrWhiteSpace(filePath)){
			return;
		}

		var directory = Path.GetDirectoryName(filePath);
		if (!string.IsNullOrEmpty(directory)) {
			Directory.CreateDirectory(directory);
		}

		if (!File.Exists(filePath)) {
			// 创建空文件，使用 FileStream 立即释放资源
			using (File.Create(filePath)) { }
		}
	}
}
