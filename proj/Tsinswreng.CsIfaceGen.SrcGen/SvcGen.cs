namespace Tsinswreng.CsIfaceGen.SrcGen;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using System.Threading.Tasks;


public class CtxCodeGenResult{
	public IArgIfaceGen ArgIfaceGen{get;set;}
	public str Code{get;set;} = "";
	public str HintName{get;set;} = "";
}

public class CfgGen{
	//public str? ProjectDir{get;set;}
	public Func<CtxCodeGenResult, CT, Task<nil>>? FnAddSrc{get;set;}
}

public class SvcGen {
	public async Task<nil> ExeAsy(
		Compilation Compilation
		,CfgGen DtoGen
		,CT Ct
	){

		var allTypes = Compilation.SourceModule.GlobalNamespace
			.GetAllNamedTypes();

		// 拿到所有类型（含嵌套）后，再收集特性
		var ifaceGenAttrs = allTypes
		.SelectMany(t => t.GetAttributes())
		.Where(ad => ad.AttributeClass?.Name == nameof(IfaceGen));

		// 2) 遍历每个 IfaceGenAttribute 实例
		foreach (var attr in ifaceGenAttrs) {
			var AttrArg = GetArg(attr);
			var parentType = AttrArg.ParentType;
			var template = AttrArg.Template;
			var FullTypePh = AttrArg.PhFullType;
			var IdFullTypePh = AttrArg.PhIdentifierSafeFullType;;


			// 3) 找出 ParentType 的所有子类型
			var subTypes = allTypes
				.Where(t => IsSubType(t, parentType, Compilation))
			;

			// 4) 为每个子类型生成代码
			foreach (var sub in subTypes) {
				//var fullName = GetFullyQualifiedName(sub);
				var fullName = ToolSrcGen.ResolveFullTypeFitsTypeof(sub);
				var idFullType = ToolGen.ToIdentifierSafeFullType(fullName);
				var sourceText = template.Replace(FullTypePh, fullName);
				sourceText = sourceText.Replace(IdFullTypePh, idFullType);

				var hintName = $"{sub.Name}_{parentType.Name}_Gen_{GetSha256HashBase64Url(template)}.g.cs";
				var Result = new CtxCodeGenResult{
					ArgIfaceGen = AttrArg,
					Code = sourceText,
					HintName = hintName,
				};
				if(DtoGen.FnAddSrc != null){
					await DtoGen.FnAddSrc(Result, Ct);
				}
			}
		}
		return NIL;
	}

	// ------------------------ 辅助方法 ------------------------

	public static str CombinePath(str Path1, str Path2){
		if(Path1.EndsWith("\\") || Path1.EndsWith("/")){
			return Path1 + Path2;
		}else{
			return Path1 + "/" + Path2;
		}
	}


	static IArgIfaceGen GetArg(
		AttributeData Attr
	){
		IArgIfaceGen R = new ArgIfaceGen();
		foreach (var namedArg in Attr.NamedArguments){
			var name = namedArg.Key;
			var value = namedArg.Value.Value;
			if(name == nameof(IfaceGen.PhFullType)){
				R.PhFullType = value?.ToString()??"";
			}else if(name == nameof(IfaceGen.Template)){
				R.Template = value?.ToString()??"";
			}else if(name == nameof(IfaceGen.ParentType)){
				if(value is INamedTypeSymbol ns){
					R.ParentType = ns;
				}
			}else if(name == nameof(IfaceGen.PhIdentifierSafeFullType)){
				R.PhIdentifierSafeFullType = value?.ToString()??"";
			}else if(name == nameof(IfaceGen.OutDir)){
				R.OutDir = value?.ToString()??"";
			}
		}
		return R;
	}


	// static bool GetArg(
	// 	AttributeData Attr
	// 	,out INamedTypeSymbol ParentType
	// 	,out str Template
	// 	,out str? FullTypePh
	// 	,out str? IdentifierSafeFullTypePh
	// ){
	// 	ParentType = null!;
	// 	Template = "";
	// 	FullTypePh = null;
	// 	IdentifierSafeFullTypePh = null;

	// 	foreach (var namedArg in Attr.NamedArguments){
	// 		var name = namedArg.Key;
	// 		var value = namedArg.Value.Value;
	// 		if(name == nameof(IfaceGen.PhFullType)){
	// 			FullTypePh = value?.ToString()??"";
	// 		}else if(name == nameof(IfaceGen.Template)){
	// 			Template = value?.ToString()??"";
	// 		}else if(name == nameof(IfaceGen.ParentType)){
	// 			if(value is INamedTypeSymbol ns){
	// 				ParentType = ns;
	// 			}
	// 		}else if(name == nameof(IfaceGen.PhIdentifierSafeFullType)){
	// 			IdentifierSafeFullTypePh = value?.ToString()??"";
	// 		}
	// 	}
	// 	return true;
	// }

	private static bool IsSubType(
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

	static string GetSha256HashBase64Url(string input){
		using var sha256 = SHA256.Create();
		byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

		// 普通 Base64 → URL 安全 Base64
		string base64 = Convert.ToBase64String(hash)
			.Replace('+', '-')
			.Replace('/', '_')
			.TrimEnd('=')
		;
		return base64;
	}

	static string GetSha256HashHex(string input){
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


}


//TODO 抽取至共ʹ庫
internal static class SymbolExtensions{
	/// <summary>
	/// 递归拿到某个命名空间下所有命名类型（包括嵌套命名空间、嵌套类型）。
	/// </summary>
	internal static IEnumerable<INamedTypeSymbol> GetAllNamedTypes(this INamespaceSymbol ns){
		foreach (var member in ns.GetMembers()){
			switch (member){
				case INamespaceSymbol nestedNs:
					foreach (var t in nestedNs.GetAllNamedTypes()){
						yield return t;
					}
				break;

				case INamedTypeSymbol type:
					yield return type;
					// 再往下递归嵌套类型
					foreach (var nested in type.GetAllNestedTypes()){
						yield return nested;
					}
				break;
			}
		}
	}

	/// <summary>
	/// 递归拿到某个类型及其所有嵌套类型。
	/// </summary>
	internal static IEnumerable<INamedTypeSymbol> GetAllNestedTypes(this INamedTypeSymbol type){
		foreach (var nested in type.GetTypeMembers()){
			yield return nested;
			foreach (var deeper in nested.GetAllNestedTypes()){
				yield return deeper;
			}
		}
	}
}


//從Tsinswreng.CsSrcGenTools複製來
class ToolSrcGen{
	public static str ResolveFullTypeFitsTypeof(ITypeSymbol T){
		//if (T == null) return string.Empty;
		if (T.IsValueType){
			// 判断是否是 Nullable<T>
			if (T.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T){
				var namedType = (INamedTypeSymbol)T;
				var innerType = namedType.TypeArguments[0];
				// 返回可空值类型比如 "System.Int32?"
				return innerType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) + "?";
			}else{
				// 普通值类型，比如 "System.Int32"
				return T.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			}
		}//~if (T.IsValueType)
		else{
			// 引用类型，不加问号，比如 "System.String"
			return T.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
		}
	}
}
