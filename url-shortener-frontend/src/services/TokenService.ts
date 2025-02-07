class TokenService {
    private static readonly TOKEN_KEY = "token";

    static getToken(): string | null {
        return localStorage.getItem(this.TOKEN_KEY);
    }

    static saveToken(token: string): void {
        localStorage.setItem(this.TOKEN_KEY, token);
    }

    static removeToken(): void {
        localStorage.removeItem(this.TOKEN_KEY);
    }

    static hasToken(): boolean {
        return !!this.getToken();
    }
}

export default TokenService;
