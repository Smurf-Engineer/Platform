import fs from "fs";
import path from "path";
import { GenerateReactRouterOptions, generateReactRouter } from "./generateReactRouter";

export type StyleConvention = "nextjs/app"; // | "nextjs/pages" | "remix/?";

export type ClientFilesystemRouterOptions = {
  /**
   * The directory to use as the root of the filesystem router.
   * @default "app"
   * @example "src/app"
   */
  dir?: string;
  /**
   * The router style convention to use.
   * @default "nextjs/app"
   */
  style?: StyleConvention;
  /**
   * The prefix to use for the import path.
   * @default "@"
   */
  importPrefix?: string;
  /**
   * The prefix to use for the asset path.
   * @default ""
   */
  assetPrefix?: string;
  /**
   * The origin to use for the asset path.
   * @default "auto"
   * @see Not yet implemented
   */
  origin?: string;
  /**
   * Limit the pages to those with particular file extensions.
   * @default [".tsx", ".ts", ".jsx", ".js"]
   * @see Not yet implemented
   */
  fileExtensions?: string[];
};

export class ClientFilesystemRouter {
  private generateOptions: GenerateReactRouterOptions;

  constructor({
    dir = "app",
    // style = "nextjs/app",
    importPrefix = "@",
    assetPrefix,
  }: ClientFilesystemRouterOptions = {}) {
    this.generateOptions = {
      appPath: path.join(process.cwd(), dir),
      importPrefix,
      assetPrefix,
    };

    if (!fs.existsSync(this.generateOptions.appPath)) {
      throw new Error(`Could not find app directory at "${this.generateOptions.appPath}".`);
    }
  }

  public generate() {
    try {
      return generateReactRouter(this.generateOptions);
    } catch (error) {
      console.error("Failed to generate valid router Javascript.");
      console.error(error);
      process.exit(1);
    }
  }
}
