import BaseApiService from "./BaseApiService";

interface ShortenRequest {
  originalUrl: string;
  description: string;
}

interface Url {
  shortUrl: string;
  originalUrl: string;
  description: string;
  createdByUserId: number;
  createdDate: string;
}

class UrlService extends BaseApiService {
  public async shortenUrl(shortenRequest: ShortenRequest): Promise<Url> {
    return await this.post<Url>("/url/shorten", shortenRequest);
  }

  public async deleteUrl(shortUrl: string): Promise<void> {
    await this.delete(`/url/mydomain.com%2F${shortUrl}`);
  }

  public async getMyUrls(): Promise<Url[]> {
    return await this.get<Url[]>("/url/my");
  }

  public async getUrlInfo(shortUrl: string): Promise<Url> {
    return await this.get<Url>(`/url/mydomain.com%2F${shortUrl}/info`);
  }

  public async getAllUrls(): Promise<Url[]> {
    return await this.get<Url[]>("/url/all");
  }
}

export default UrlService;
