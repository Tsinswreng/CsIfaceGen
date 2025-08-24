=


= 例
```cs

//此特性爲源生成器庫提供
public class IfaceGen:Attribute{
	public IfaceGen(
	){

	}
	public Type ParentType{get;set;}
	public str TypePlaceholder{get;set;}
	public str Template{get;set;}
}


//以下代碼皆爲用戶項目中定義的、不是源生成器庫提供
public interface IAppSerializable{

}


[IfaceGen(
	ParentType = typeof(IAppSerializable)
	,TypePlaceholder = "TYPE"
	,Template =
"""
namespace Ngaq.Core.Infra{
	[global::System.Text.Json.Serialization.JsonSerializableAttribute(
		typeof(TYPE)
	)]
	public partial class AppJsonCtx{

	}
}
"""
)]
public interface IIfaceAttrCfg{

}

namespace Ngaq.Core.Infra{
	public partial class AppJsonCtx: JsonSerializerContext{}
}


namespace App.A{
	public class MyEntityA: IAppSerializable{}
}


```

生成代碼:
```cs
namespace Ngaq.Core.Infra{
	[global::System.Text.Json.Serialization.JsonSerializableAttribute(
		typeof(global::App.A.MyEntityA)
	)]
	public partial class AppJsonCtx{

	}
}
```
解釋: 若一個類型TargetType是IfaceGen.ParentType的子類型(ParentType可以是接口或父類)、則爲TargetType生成Template中的代碼、並將TypePlaceholder替換爲TargetType的全名(global::開頭)

注意: IfaceGen特性可以在一個類型(接口, 類, 結構體等)上聲明多次 傳不同參數
也可以有在多個類型上聲明多次IfaceGen特性

//用c\# 實現。





= 構建命令行工具

```bash
# pwd=Tsinswreng.CsIfaceGen.Cli/
dotnet publish -c Release -r win-x64
```
