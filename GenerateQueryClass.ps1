# Définition générique de la structure en entrée

# param (
    # [string]$struct_name = "Client",
    # [array]$props = @(
        # @("int", "Id", $true), 
		# @("bool", "IsActive", $true), 
        # @("string", "Name", $true)
    # )
# )

param (
    [string]$struct_name = "Employee",
    [array]$props = @(
        @("int", "Id", $true), 
		@("bool", "IsActive", $true), 
        @("int", "ClientId", $true), 
        @("string", "FirstName", $true), 
        @("string", "LastName", $true), 
        @("string", "Email", $true), 
        @("string", "Phone", $true), 
        @("string", "SpecialNote", $true)
    )
)

$class_name = "${struct_name}"
$output = "public class ${class_name}s : Base`n{"  

# Génération des propriétés
foreach ($prop in $props) {
    $type = $prop[0]
    $name = $prop[1]
    $nullable = $prop[2]
    $listType = "List<$type>"
    if ($nullable) {
        $listType = "List<$type>?"
    }
    $output += "`n    private $listType ${name}s;"
}

$output += "`n`n    public ${class_name}s() : base(`"$(($struct_name).ToLower())s`")`n    {"
foreach ($prop in $props) {
    if (-not $prop[2]) {
		$name = $prop[0]
		$type = $prop[1]
        $output += "`n        ${type}s = new List<${name}>();"
    }
}
$output += "`n    }"

$output += "`n`n    public ActivAndZen.Model.$struct_name this[int index]`n    {`n        get`n        {`n            if (index > _count || index < 0)`n                throw new IndexOutOfRangeException(`"Index invalide.`");`n            return new ActivAndZen.Model.$struct_name {"

foreach ($prop in $props) {
    $name = $prop[1]
    if ($prop[2]) {
        $output += "`n                $name = ${name}s != null ? ${name}s[index] : null,"
    } else {
        $output += "`n                $name = ${name}s[index],"
    }
}
$output = $output.TrimEnd(",") + "`n            };`n        }`n    }"

$output += "`n`n    protected override void GetLine(SqliteDataReader dataReader)`n    {"
$index = 0
foreach ($prop in $props) {
    $name = $prop[1]
	$getters = @(@("int", "GetInt32"), @("string", "GetString"), @("bool", "GetBoolean"))
    $getter = ""
	
	foreach ($g in $getters) {
		if ($prop[0] -eq $g[0]) {
			$getter = $g[1]
		}
	}
	
    if ($prop[2]) {
        $output += "`n        if (${name}s != null) ${name}s.Add(dataReader.$getter($index));"
    } else {
        $output += "`n        ${name}s.Add(dataReader.$getter($index));"
    }
    $index++
}
$output += "`n    }"

$output += "`n`n    protected override void Clear()`n    {"
foreach ($prop in $props) {
    $name = $prop[1]
    if ($prop[2]) {
        $output += "`n        if (${name}s != null) ${name}s.Clear();"
    } else {
        $output += "`n        ${name}s.Clear();"
    }
}
$output += "`n    }`n}"

# Write-Host $output
$output | Set-Clipboard