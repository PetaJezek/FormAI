# Count lines of C# code in current directory and subdirectories
$files = Get-ChildItem -Path . -Recurse -Include *.cs
$total = 0
$fileCount = 0

# Define patterns for files to exclude (generated and temporary files)
$excludePatterns = @(
    "*AssemblyInfo.cs",
    "*GlobalUsings.g.cs", 
    "*_wpftmp*",
    "*.g.cs",
    "*AssemblyAttributes.cs",
    "*_Content.g.cs"
)

# Track statistics
$stats = @{
    TotalFiles = 0
    TotalLines = 0
    CodeLines = 0
    BlankLines = 0
    CommentLines = 0
}

foreach ($file in $files) {
    # Skip generated and temporary files
    $shouldExclude = $false
    foreach ($pattern in $excludePatterns) {
        if ($file.Name -like $pattern) {
            $shouldExclude = $true
            break
        }
    }
    
    if ($shouldExclude) {
        Write-Host "Skipped (generated): $($file.Name)" -ForegroundColor DarkGray
        continue
    }
    
    try {
        $content = Get-Content $file.FullName -ErrorAction Stop
        $fileCount++
        $stats.TotalFiles++
        
        $codeLines = 0
        $inBlockComment = $false
        
        foreach ($line in $content) {
            $stats.TotalLines++
            $trimmedLine = $line.Trim()
            
            # Check for blank lines
            if ($trimmedLine -eq "") {
                $stats.BlankLines++
                continue
            }
            
            # Handle block comments (/* */)
            if ($inBlockComment) {
                $stats.CommentLines++
                if ($trimmedLine -match '\*/') {
                    $inBlockComment = $false
                    # Check if there's code after the block comment on the same line
                    $afterComment = $trimmedLine -replace '^.*?\*/', ''
                    if ($afterComment.Trim() -ne "") {
                        $codeLines++
                        $stats.CodeLines++
                    }
                }
                continue
            }
            
            # Check for start of block comment
            if ($trimmedLine -match '/\*') {
                $stats.CommentLines++
                $inBlockComment = $true
                # Check if block comment ends on the same line
                if ($trimmedLine -match '\*/') {
                    $inBlockComment = $false
                }
                # Check if there's code before the block comment
                $beforeComment = $trimmedLine -replace '/\*.*', ''
                if ($beforeComment.Trim() -ne "") {
                    $codeLines++
                    $stats.CodeLines++
                }
                continue
            }
            
            # Check for single-line comments
            if ($trimmedLine.StartsWith("//")) {
                $stats.CommentLines++
                continue
            }
            
            # Check for lines with code followed by comments
            if ($trimmedLine -match '^.+//') {
                $codeLines++
                $stats.CodeLines++
                continue
            }
            
            # This is a code line
            $codeLines++
            $stats.CodeLines++
        }
        
        $total += $codeLines
        Write-Host "Processed: $($file.Name) - $codeLines lines of code" -ForegroundColor Gray
        
    } catch {
        Write-Warning "Could not read file: $($file.FullName) - $($_.Exception.Message)"
    }
}

# Display results
Write-Host "`n=== C# Code Statistics ===" -ForegroundColor Cyan
Write-Host "Files processed: $($stats.TotalFiles)" -ForegroundColor Green
Write-Host "Total lines: $($stats.TotalLines)" -ForegroundColor Yellow
Write-Host "Code lines: $($stats.CodeLines)" -ForegroundColor Green
Write-Host "Comment lines: $($stats.CommentLines)" -ForegroundColor Blue
Write-Host "Blank lines: $($stats.BlankLines)" -ForegroundColor Gray
Write-Host "`nLines of code (excluding comments and blanks): $total" -ForegroundColor Magenta

if ($stats.TotalFiles -eq 0) {
    Write-Host "No C# files found in the current directory and subdirectories." -ForegroundColor Red
}