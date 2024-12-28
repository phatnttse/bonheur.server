# Dùng image SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


# Copy các file csproj của tất cả các dự án vào container và restore các thư viện NuGet
COPY ["Bonheur.Services/Bonheur.Services.csproj", "Bonheur.Services/"]
COPY ["Bonheur.BusinessObjects/Bonheur.BusinessObjects.csproj", "Bonheur.BusinessObjects/"]
COPY ["Bonheur.DAOs/Bonheur.DAOs.csproj", "Bonheur.DAOs/"]
COPY ["Bonheur.API/Bonheur.API.csproj", "Bonheur.API/"]
COPY ["Bonheur.Utils/Bonheur.Utils.csproj", "Bonheur.Utils/"]
COPY ["Bonheur.Repositories/Bonheur.Repositories.csproj", "Bonheur.Repositories/"]

# Restore các thư viện NuGet
RUN dotnet restore "Bonheur.API/Bonheur.API.csproj"

# Copy toàn bộ mã nguồn vào container
COPY . . 

# Build ứng dụng API ở chế độ Release
RUN dotnet build "Bonheur.API/Bonheur.API.csproj" -c Release -o /app/build

# Publish ứng dụng API vào thư mục /app/publish
RUN dotnet publish "Bonheur.API/Bonheur.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Tạo image cuối cùng để chạy ứng dụng API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy kết quả publish từ build container vào final container
COPY --from=build /app/publish .

# Thiết lập entrypoint cho container
ENTRYPOINT ["dotnet", "Bonheur.API.dll"]

# Expose các cổng ứng dụng
EXPOSE 80
