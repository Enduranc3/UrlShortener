import BaseApiService from "./BaseApiService";
import TokenService from "./TokenService";

interface User {
    id: string;
    email: string;
    username: string;
    role: string;
}

interface UserRegisterRequest {
    email: string;
    password: string;
    username: string;
}

interface UserLoginRequest {
    email: string;
    password: string;
}

class AuthService extends BaseApiService {
    public async register(
        userRegisterRequest: UserRegisterRequest
    ): Promise<string> {
        await this.post<void>("/authorization/register", userRegisterRequest);
        return userRegisterRequest.email;
    }

    public async login({email, password}: UserLoginRequest): Promise<string> {
        const response = await this.post<string>("/authorization/login", {
            email,
            password,
        });
        const token = response;
        TokenService.saveToken(token);
        return token;
    }

    public async getCurrentUser(): Promise<User> {
        const response = await this.get<User>("/authorization/my");
        return response;
    }

    public logout(): void {
        TokenService.removeToken();
    }

    public isAuthenticated(): boolean {
        return TokenService.hasToken();
    }
}

export default AuthService;
